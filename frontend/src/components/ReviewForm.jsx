import React, { useState, useEffect } from 'react';
import { Form, Input, Rate, Button, message, Select, Empty } from 'antd';
import { reviewService } from '../services/reviewService';
import axios from 'axios';
import { API_CONFIG } from '../config';

const { TextArea } = Input;
const { Option } = Select;

const ReviewForm = ({ roomId, onSuccess }) => {
  const [form] = Form.useForm();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchUserBookings = async () => {
      try {
        const token = localStorage.getItem('token');
        if (!token) return;

        const response = await axios.get(
          `${API_CONFIG.API_BASE_URL}${API_CONFIG.ENDPOINTS.BOOKINGS.USER_ROOM_BOOKINGS(roomId)}`,
          {
            headers: {
              'Authorization': `Bearer ${token}`
            }
          }
        );

        // Фильтруем только те бронирования, по которым можно оставить отзыв
        const availableBookings = response.data.filter(booking => booking.canReview);
        setBookings(availableBookings);
      } catch (error) {
        console.error('Error fetching bookings:', error);
        message.error('Не удалось загрузить ваши бронирования');
      }
    };

    fetchUserBookings();
  }, [roomId]);

  const handleSubmit = async (values) => {
    try {
      setLoading(true);
      await reviewService.createReview({
        bookingId: values.bookingId,
        rating: values.rating,
        comment: values.comment
      });

      message.success('Спасибо за ваш отзыв! Он будет опубликован после модерации.');
      form.resetFields();
      if (onSuccess) onSuccess();
    } catch (error) {
      console.error('Error submitting review:', error);
      message.error('Не удалось отправить отзыв. Пожалуйста, попробуйте позже.');
    } finally {
      setLoading(false);
    }
  };

  if (bookings.length === 0) {
    return (
      <Empty
        description={
          <span>
            Вы можете оставить отзыв только после завершения проживания.<br />
            Для этого необходимо иметь завершенное бронирование этого номера.
          </span>
        }
      />
    );
  }

  return (
    <Form
      form={form}
      onFinish={handleSubmit}
      layout="vertical"
      className="review-form"
    >
      <Form.Item
        name="bookingId"
        label="Выберите период проживания"
        rules={[{ required: true, message: 'Пожалуйста, выберите бронирование' }]}
      >
        <Select placeholder="Выберите период проживания">
          {bookings.map(booking => (
            <Option key={booking.id} value={booking.id}>
              {new Date(booking.checkInDate).toLocaleDateString()} - {new Date(booking.checkOutDate).toLocaleDateString()}
            </Option>
          ))}
        </Select>
      </Form.Item>

      <Form.Item
        name="rating"
        label="Ваша оценка"
        rules={[{ required: true, message: 'Пожалуйста, поставьте оценку' }]}
      >
        <Rate />
      </Form.Item>

      <Form.Item
        name="comment"
        label="Ваш отзыв"
        rules={[
          { required: true, message: 'Пожалуйста, напишите отзыв' },
          { min: 10, message: 'Отзыв должен содержать минимум 10 символов' }
        ]}
      >
        <TextArea
          rows={4}
          placeholder="Расскажите о вашем опыте проживания..."
          maxLength={1000}
          showCount
        />
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" loading={loading} block>
          Отправить отзыв
        </Button>
      </Form.Item>
    </Form>
  );
};

export default ReviewForm;