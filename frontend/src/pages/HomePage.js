import React, { useState, useEffect, useRef, useCallback, useMemo } from "react";
import "./HomePage.css";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { API_CONFIG } from '../config';
import RoomFilter from '../components/RoomFilter';

const HomePage = ({ token }) => {
  const videoRef = useRef(null);
  const navigate = useNavigate();
  
  const [rooms, setRooms] = useState([]);
  const [currentImageIndexes, setCurrentImageIndexes] = useState({});
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchRooms = useCallback(async (filters = {}) => {
    try {
      setLoading(true);
      setError(null);
      
      // Создаем параметры запроса
      const params = new URLSearchParams({
        page: currentPage,
        pageSize: 12
      });

      // Добавляем базовые фильтры
      if (filters.minPrice) params.append('minPrice', filters.minPrice);
      if (filters.maxPrice) params.append('maxPrice', filters.maxPrice);
      if (filters.roomType && filters.roomType !== 'all') params.append('roomType', filters.roomType);
      if (filters.minCapacity) params.append('minCapacity', filters.minCapacity);
      if (filters.maxCapacity) params.append('maxCapacity', filters.maxCapacity);
      if (filters.floor) params.append('floor', filters.floor);

      let endpoint = `${API_CONFIG.API_BASE_URL}/room/filter`;
      
      // Если указаны даты, сначала проверяем доступность
      if (filters.checkInDate && filters.checkOutDate) {
        const checkInDate = new Date(filters.checkInDate);
        const checkOutDate = new Date(filters.checkOutDate);
        
        console.log('Original dates:', {
          checkInDate: filters.checkInDate,
          checkOutDate: filters.checkOutDate
        });
        
        checkInDate.setHours(0, 0, 0, 0);
        checkOutDate.setHours(0, 0, 0, 0);
        
        // Форматируем даты в локальном формате YYYY-MM-DD
        const formatDate = (date) => {
          const year = date.getFullYear();
          const month = String(date.getMonth() + 1).padStart(2, '0');
          const day = String(date.getDate()).padStart(2, '0');
          return `${year}-${month}-${day}`;
        };

        const checkInStr = formatDate(checkInDate);
        const checkOutStr = formatDate(checkOutDate);
        
        console.log('Checking availability for dates:', { checkInStr, checkOutStr });
        
        // Получаем список занятых номеров
        const availabilityResponse = await axios.get(
          `${API_CONFIG.API_BASE_URL}/room/availability?checkIn=${checkInStr}&checkOut=${checkOutStr}`
        );
        
        if (availabilityResponse.data && Array.isArray(availabilityResponse.data)) {
          // Добавляем список занятых номеров в параметры запроса
          params.append('excludeRoomIds', availabilityResponse.data.join(','));
        }
      }

      console.log('Final request URL:', `${endpoint}?${params.toString()}`);
      const response = await axios.get(`${endpoint}?${params.toString()}`);
      
      if (!response.data || !Array.isArray(response.data)) {
        throw new Error('Неверный формат данных от сервера');
      }

      // Преобразуем данные для отображения
      const processedRooms = response.data.map(room => ({
        ...room,
        rating: room.averageRating || 0,
        reviewsCount: room.reviewCount || 0
      }));

      // Инициализируем индексы только для новых комнат
      setCurrentImageIndexes(prev => {
        const newIndexes = { ...prev };
        processedRooms.forEach(room => {
          if (!(room.id in newIndexes)) {
            newIndexes[room.id] = 0;
          }
        });
        return newIndexes;
      });

      setRooms(processedRooms);
      
      const totalCount = parseInt(response.headers['x-total-count']) || response.data.length;
      setTotalItems(totalCount);
      setTotalPages(Math.ceil(totalCount / 12));
      
    } catch (err) {
      console.error('Error fetching rooms:', err);
      setError(err.response?.data?.error || 'Ошибка при загрузке номеров');
      setRooms([]);
    } finally {
      setLoading(false);
    }
  }, [currentPage]); // Убрали currentImageIndexes из зависимостей

  useEffect(() => {
    fetchRooms();
  }, [currentPage, fetchRooms]);

  const handleImageHover = useCallback((roomId, direction) => {
    setCurrentImageIndexes((prev) => {
      const currentIndex = prev[roomId] || 0;
      const room = rooms.find((r) => r.id === roomId);
      
      if (!room?.photos?.length) return prev;

      const newIndex = direction === "next"
        ? (currentIndex + 1) % room.photos.length
        : (currentIndex - 1 + room.photos.length) % room.photos.length;

      return { ...prev, [roomId]: newIndex };
    });
  }, [rooms]);

  const handleBookRoom = useCallback((roomId) => {
    if (!token) {
      navigate('/auth');
      return;
    }
    navigate(`/booking/${roomId}`);
  }, [token, navigate]);

  const handleFilter = useCallback((filters) => {
    setCurrentPage(1);
    fetchRooms(filters);
  }, [fetchRooms]);

  const RoomTypeBadge = useMemo(() => ({ type }) => {
    const typeNames = {
      'Standard': "Стандарт",
      'Superior': "Стандарт плюс",
      'FamilyRoom': "Семейный",
      'Deluxe': "Делюкс",
      'Suite': "Люкс"
    };
    
    const displayName = typeNames[type] || "Неизвестный тип";
    const className = displayName.toLowerCase().replace(/\s+/g, '-');
    
    return (
      <span className={`room-type-badge ${className}`}>
        {displayName}
      </span>
    );
  }, []);

  const RoomCapacityBadge = useMemo(() => ({ capacity }) => {
    return (
      <span className="room-capacity-badge">
        {capacity} {capacity === 1 ? 'человек' : capacity < 5 ? 'человека' : 'человек'}
      </span>
    );
  }, []);

  const renderStars = useCallback((rating) => {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

    return (
      <div className="stars">
        {'★'.repeat(fullStars)}
        {hasHalfStar && '½'}
        {'☆'.repeat(emptyStars)}
      </div>
    );
  }, []);

  // Мемоизируем рендер карточки комнаты
  const RoomCard = useMemo(() => ({ room }) => (
    <div key={room.id} className="room-card">
      <div
        className="room-image-container"
        onClick={() => navigate(`/room/${room.id}`)}
        style={{ cursor: 'pointer' }}
      >
        <img
          src={
            room.photos && room.photos.length > 0
              ? `${API_CONFIG.SERVER_BASE_URL}${room.photos[currentImageIndexes[room.id] || 0].photoUrl}`
              : `${API_CONFIG.SERVER_BASE_URL}/uploads/placeholder.jpg`
          }
          alt={`Номер ${room.roomNumber}`}
          className="room-image"
          onError={(e) => {
            e.target.onerror = null;
            e.target.src = `${API_CONFIG.SERVER_BASE_URL}/uploads/placeholder.jpg`;
          }}
        />
        {room.photos && room.photos.length > 1 && (
          <div className="image-nav">
            <button
              className="nav-button prev"
              onClick={(e) => {
                e.stopPropagation();
                handleImageHover(room.id, "prev");
              }}
            >
              &lt;
            </button>
            <button
              className="nav-button next"
              onClick={(e) => {
                e.stopPropagation();
                handleImageHover(room.id, "next");
              }}
            >
              &gt;
            </button>
          </div>
        )}
      </div>

      <div className="room-info">
        <h3>Номер {room.roomNumber || 'б/н'}</h3>
        <div className="room-type">
          <RoomTypeBadge type={room.roomType} />
          <RoomCapacityBadge capacity={room.capacity || 1} />
        </div>
        <div className="room-rating">
          {renderStars(room.rating || 0)}
          <span>({room.reviewsCount || 0} отзывов)</span>
        </div>
        <div className="room-price">
          {(room.roomCharge || 0).toLocaleString('ru-RU')} ₽ / ночь
        </div>
        <button
          className="book-button"
          onClick={(e) => {
            e.stopPropagation();
            handleBookRoom(room.id);
          }}
        >
          Забронировать
        </button>
      </div>
    </div>
  ), [currentImageIndexes, handleImageHover, handleBookRoom, navigate, RoomTypeBadge, RoomCapacityBadge, renderStars]);

  return (
    <div className="hotel-page">
      {/* Приветственная шапка */}
      <div className="welcome-header">
        <video ref={videoRef} autoPlay loop muted playsInline className="welcome-media">
          <source src={`${API_CONFIG.SERVER_BASE_URL}/uploads/main_pano.mp4`} type="video/mp4" />
          <img src={`${API_CONFIG.SERVER_BASE_URL}/uploads/main.jpg`} alt="Отель" className="welcome-media" />
        </video>
        
        <div className="welcome-content">
          <h1>Добро пожаловать в Zenith Hotel!</h1>
          <p>Почувствуйте комфорт и роскошь, наслаждаясь незабываемым отдыхом в самом сердце города.</p>
        </div>
      </div>

      <h1>Номера нашего отеля</h1>
      
      <RoomFilter onFilter={handleFilter} />

      {loading && <div className="loading">Загрузка номеров...</div>}
      {error && <div className="error">{error}</div>}
      
      <div className="rooms-grid">
        {rooms.length > 0 ? (
          rooms.map(room => <RoomCard key={room.id} room={room} />)
        ) : (
          <div className="no-rooms">
            {error ? 'Произошла ошибка при загрузке номеров' : 'Нет доступных номеров по заданным критериям'}
          </div>
        )}
      </div>

      {totalPages > 1 && (
        <div className="pagination">
          <button
            className="pagination-button"
            onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
            disabled={currentPage === 1}
          >
            Назад
          </button>
          <span>
            Страница {currentPage} из {totalPages}
          </span>
          <button
            className="pagination-button"
            onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
            disabled={currentPage === totalPages}
          >
            Вперед
          </button>
        </div>
      )}
    </div>
  );
};

export default HomePage;
