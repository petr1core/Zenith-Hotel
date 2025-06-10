import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_CONFIG } from '../config';
import { Rate } from 'antd';
import './RoomPage.css';
import ReviewList from '../components/ReviewList';
import ReviewForm from '../components/ReviewForm';
import '../components/Reviews.css';

const RoomPage = ({ token }) => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [room, setRoom] = useState(null);
    const [currentImageIndex, setCurrentImageIndex] = useState(0);
    const [showReviewForm, setShowReviewForm] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchRoom = useCallback(async () => {
        try {
            setLoading(true);
            // Получаем основную информацию о номере
            const response = await axios.get(`${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.ROOMS.DETAILS(id)}`);
            const roomData = response.data;

            // Фотографии и удобства уже включены в основной ответ
            setRoom(roomData);
            console.log('Received room data:', roomData);
        } catch (error) {
            console.error('Error fetching room:', error);
            setError('Ошибка при загрузке данных номера');
        } finally {
            setLoading(false);
        }
    }, [id]);

    useEffect(() => {
        fetchRoom();
    }, [fetchRoom]);

    const handleImageChange = (index) => {
        setCurrentImageIndex(index);
    };

    const handleBookRoom = async () => {
        if (!token) {
            navigate('/auth', { state: { returnUrl: `/room/${id}` } });
            return;
        }

        try {
            const response = await axios.get(`${API_CONFIG.API_BASE_URL}/auth/validate`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            const { isValid } = response.data;

            if (!isValid) {
                localStorage.removeItem('token');
                localStorage.removeItem('role');
                navigate('/auth', { state: { returnUrl: `/room/${id}` } });
                return;
            }

            navigate(`/booking/${id}`);
        } catch (error) {
            console.error('Error validating token:', error);
            localStorage.removeItem('token');
            localStorage.removeItem('role');
            navigate('/auth', { state: { returnUrl: `/room/${id}` } });
        }
    };

    const handleReviewSuccess = () => {
        setShowReviewForm(false);
        // Обновляем список отзывов
        fetchRoom();
    };

    const getRoomTypeName = (type) => {
        const typeNames = {
            'Standard': "Стандарт",
            'Superior': "Стандарт плюс",
            'FamilyRoom': "Семейный",
            'Deluxe': "Делюкс",
            'Suite': "Люкс"
        };
        return typeNames[type] || "Неизвестный тип";
    };

    if (loading) return <div className="loading">Загрузка...</div>;
    if (error) return <div className="error">{error}</div>;
    if (!room) return <div className="error">Номер не найден</div>;

    return (
        <div className="room-page">
            <div className="room-page-header">
                <h2>Номер {room.roomNumber}</h2>
                <div className="header-details">
                    <div className="stars">
                        <Rate disabled allowHalf value={room.averageRating || 0} />
                    </div>
                    <div style={{ marginLeft: 8, fontSize: '1rem', color: '#c0c5ce' }}>
                        ({(room.averageRating || 0).toFixed(1)})
                    </div>
                    <a href="#reviews" className="reviews-link">{(room.reviewCount || 0)} отзывов</a>
                </div>
            </div>

            <div className="room-page-layout">
                {/* Левая колонка */}
                <div className="main-content">
                    <div className="gallery">
                        <div className="main-image">
                            {room.photos && room.photos.length > 0 ? (
                                <img
                                    src={`${API_CONFIG.SERVER_BASE_URL}${room.photos[currentImageIndex].photoUrl}`}
                                    alt="Основное фото номера"
                                />
                            ) : (
                                <div className="no-image">Нет фото</div>
                            )}
                        </div>
                        <div className="thumbnails">
                            {room.photos?.map((photo, index) => (
                                <div
                                    key={photo.id}
                                    className={`thumbnail ${currentImageIndex === index ? 'active' : ''}`}
                                    onClick={() => handleImageChange(index)}
                                >
                                    <img
                                        src={`${API_CONFIG.SERVER_BASE_URL}${photo.photoUrl}`}
                                        alt={`Фото ${index + 1}`}
                                    />
                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="reviews" id="reviews">
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
                            <h3>Отзывы</h3>
                            {token && !showReviewForm && (
                                <button
                                    className="write-review-button"
                                    onClick={() => setShowReviewForm(true)}
                                >
                                    Написать отзыв
                                </button>
                            )}
                        </div>

                        {showReviewForm && (
                            <div className="review-form-container">
                                <ReviewForm
                                    roomId={id}
                                    onSuccess={handleReviewSuccess}
                                />
                            </div>
                        )}

                        <ReviewList roomId={id} />
                    </div>
                </div>

                {/* Правая колонка */}
                <div className="sidebar">
                    <div className="booking-box">
                        <div className="price">{room.roomCharge} ₽ / ночь</div>
                        <button className="book-button" onClick={handleBookRoom}>
                            Забронировать
                        </button>

                        <div className="room-details">
                            <div className="detail-item">
                                <span className="label">Тип номера:</span>
                                <span className="value">{getRoomTypeName(room.roomType)}</span>
                            </div>
                            <div className="detail-item">
                                <span className="label">Вместимость:</span>
                                <span className="value">{room.capacity} {room.capacity === 1 ? 'человек' : room.capacity < 5 ? 'человека' : 'человек'}</span>
                            </div>
                        </div>

                        <div className="desc-box">
                            <h4>Описание</h4>
                            <p>{room.description}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default RoomPage; 