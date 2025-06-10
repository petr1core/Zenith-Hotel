import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_CONFIG } from '../config';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { ru } from 'date-fns/locale';
import './BookingPage.css';

// Enum для статусов бронирования
const BookingStatus = {
    Pending: 'Pending',
    Confirmed: 'Confirmed',
    Cancelled: 'Cancelled'
};

const BookingPage = () => {
    const { roomId } = useParams();
    const navigate = useNavigate();
    const [room, setRoom] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [startDate, setStartDate] = useState(null);
    const [endDate, setEndDate] = useState(null);
    // Заглушка для дополнительных опций. Реальные опции могут приходить с бэкенда.
    const [options, setOptions] = useState([
        { id: 1, name: 'Завтрак', price: 500, selected: false },
        { id: 2, name: 'Мини-бар', price: 1000, selected: false },
        { id: 3, name: 'Сейф', price: 1000, selected: false },
        { id: 4, name: 'Всё включено', price: 6000, selected: false }
    ]);

    useEffect(() => {
        const fetchRoom = async () => {
            try {
                const response = await axios.get(`${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.ROOMS.DETAILS(roomId)}`);
                setRoom(response.data);
            } catch (error) {
                console.error('Error fetching room details:', error);
                setError('Ошибка при загрузке данных номера');
            } finally {
                setLoading(false);
            }
        };

        if (roomId) {
            fetchRoom();
        }
    }, [roomId]);

    const handleOptionChange = (optionId) => {
        setOptions(options.map(option =>
            option.id === optionId ? { ...option, selected: !option.selected } : option
        ));
    };

    const calculateTotalPrice = () => {
        if (!room || !startDate || !endDate) return 0;
        const days = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24));
        if (days <= 0) return 0;

        let total = room.roomCharge * days;
        options.forEach(option => {
            if (option.selected) {
                if (option.id === 3 || option.id === 4) {
                    total += option.price;
                }
                else {
                    total += option.price * days; // Предполагаем, что опции оплачиваются за день
                }
            }
        });
        return total;
    };

    const handleConfirmBooking = async () => {
        if (!startDate || !endDate) {
            setError('Пожалуйста, выберите даты заезда и выезда');
            return;
        }
        if (!room) {
            setError('Ошибка: данные номера не загружены');
            return;
        }

        const token = localStorage.getItem('token');
        if (!token) {
            console.error("Booking attempt without token.");
            alert('Для бронирования необходимо авторизоваться.');
            navigate('/auth');
            return;
        }

        console.log('Token found:', token);

        try {
            const tokenPayload = JSON.parse(atob(token.split('.')[1]));
            const userId = tokenPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

            if (!userId) {
                console.error("Could not find userId in token payload.", tokenPayload);
                setError('Ошибка авторизации: не удалось определить пользователя. Пожалуйста, войдите в систему заново.');
                // Опционально: выйти из системы, если токен "плохой"
                // localStorage.removeItem('token');
                // navigate('/auth');
                return;
            }

            console.log('User ID from token:', userId);

            const bookingPayload = {
                roomId: room.id,
                checkInDate: startDate.toISOString(),
                checkOutDate: endDate.toISOString(),
                selectedServices: options.filter(opt => opt.selected).map(opt => opt.id),
                bookingStatus: 'Confirmed'
            };

            // 1. Отправляем POST запрос на создание бронирования
            const createResponse = await axios.post(`${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.BOOKINGS.CREATE}`, bookingPayload, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            console.log('Booking creation successful:', createResponse.data);

            // Показываем уведомление об успешном заказе
            alert('Заказ успешно оформлен! Ожидайте дальнейших уведомлений.');
            navigate('/'); // Перенаправляем на главную или другую страницу

        } catch (error) {
            console.error('Booking process error:', error.response?.data || error);
            setError(error.response?.data?.message || 'Произошла ошибка при оформлении бронирования.');
        }
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
    if (error && !room) return <div className="error">{error}</div>; // Показываем ошибку только если данные номера не загрузились
    if (!room) return <div className="loading">Загрузка данных номера...</div>;

    const totalPrice = calculateTotalPrice();

    return (
        <div className="booking-page">
            <h1>Бронирование номера {room.roomNumber}</h1>

            {error && <div className="booking-error">{error}</div>}

            <div className="booking-details">
                <h2>Детали номера</h2>
                <p>Тип: {getRoomTypeName(room.roomType)}</p>
                <p>Вместимость: {room.capacity} {room.capacity === 1 ? 'человек' : room.capacity < 5 ? 'человека' : 'человек'}</p>
                <p>Цена за ночь: {room.roomCharge} ₽</p>
                <p>Описание: {room.description}</p>
            </div>

            <div className="date-selection">
                <h2>Выберите даты</h2>
                <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ru}>
                    <div className="date-pickers">
                        <div>
                            <DatePicker
                                label="Дата заезда"
                                value={startDate}
                                onChange={(newValue) => setStartDate(newValue)}
                                minDate={new Date()}
                                slotProps={{ textField: { variant: 'outlined', fullWidth: true } }}
                            />
                        </div>
                        <div>
                            <DatePicker
                                label="Дата выезда"
                                value={endDate}
                                onChange={(newValue) => setEndDate(newValue)}
                                minDate={startDate || new Date()}
                                slotProps={{ textField: { variant: 'outlined', fullWidth: true } }}
                            />
                        </div>
                    </div>
                </LocalizationProvider>
            </div>

            <div className="options-selection">
                <h2>Дополнительные опции</h2>
                {options.map(option => (
                    <div key={option.id} className="option-item">
                        <input
                            type="checkbox"
                            id={`option-${option.id}`}
                            checked={option.selected}
                            onChange={() => handleOptionChange(option.id)}
                        />
                        <label htmlFor={`option-${option.id}`}>{option.name} (+{option.price} ₽)</label>
                    </div>
                ))}
            </div>

            <div className="total-price">
                <h2>Итого: {totalPrice} ₽</h2>
            </div>

            <div className="booking-actions">
                <button onClick={handleConfirmBooking} className="confirm-button">
                    Подтвердить бронирование
                </button>
            </div>
        </div>
    );
};

export default BookingPage; 