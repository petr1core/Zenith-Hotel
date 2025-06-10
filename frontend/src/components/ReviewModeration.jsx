// components/ReviewModeration.jsx
import React, { useState, useEffect } from 'react';

import { List, Card, Button, Rate, message, Spin, Empty, Input } from 'antd';
import axios from 'axios';
import { API_CONFIG } from '../config';

const { TextArea } = Input;

const ReviewModeration = () => {
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [moderatorComments, setModeratorComments] = useState({});

  const fetchPendingReviews = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const response = await axios.get(`${API_CONFIG.API_BASE_URL}/review/pending`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      setReviews(response.data);
    } catch (error) {
      console.error('Failed to load pending reviews', error);
      message.error('Не удалось загрузить отзывы для модерации');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingReviews();
  }, []);

  const handleCommentChange = (reviewId, value) => {
    setModeratorComments(prev => ({ ...prev, [reviewId]: value }));
  };

  const handleModerate = async (reviewId, newStatus) => {
    try {
      const adminEmail = JSON.parse(atob(localStorage.getItem('token').split('.')[1]))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
      const moderatorComment = moderatorComments[reviewId] || `Модерировано: ${adminEmail}`;

      await axios.put(
        `${API_CONFIG.API_BASE_URL}/review/${reviewId}/moderate`,
        { status: newStatus, moderatorComment }
      );

      message.success(`Отзыв успешно ${newStatus === 'Approved' ? 'одобрен' : 'отклонен'}.`);
      setReviews(reviews.filter(review => review.id !== reviewId));
      setModeratorComments(prev => {
        const newComments = { ...prev };
        delete newComments[reviewId];
        return newComments;
      });

    } catch (error) {
      console.error('Failed to moderate review', error);
      message.error('Ошибка при модерации отзыва');
    }
  };

  if (loading) {
    return <div style={{ textAlign: 'center', margin: '2rem 0' }}><Spin size="large" /></div>;
  }

  if (reviews.length === 0) {
    return <Empty description="Нет отзывов, ожидающих модерации" />;
  }

  return (
    <List
      grid={{ gutter: 16, column: 1 }}
      dataSource={reviews}
      renderItem={review => (
        <List.Item>
          <Card
            title={`Отзыв на номер: ${review.room.roomNumber}`}
            extra={`от ${review.user.fullName}`}
          >
            <p><strong>Дата:</strong> {new Date(review.createdAt).toLocaleDateString()}</p>
            <Rate disabled defaultValue={review.rating} />
            <p style={{ marginTop: '1rem' }}>{review.comment}</p>
            <TextArea
              rows={2}
              placeholder="Комментарий модератора (необязательно)"
              value={moderatorComments[review.id] || ''}
              onChange={(e) => handleCommentChange(review.id, e.target.value)}
              style={{ marginTop: '1rem', marginBottom: '1rem' }}
            />
            <div style={{ textAlign: 'right' }}>
              <Button
                type="primary"
                style={{ marginRight: 8 }}
                onClick={() => handleModerate(review.id, 'Approved')}
              >
                Одобрить
              </Button>
              <Button
                type="primary"
                danger
                onClick={() => handleModerate(review.id, 'Rejected')}
              >
                Отклонить
              </Button>
            </div>
          </Card>
        </List.Item>
      )}
    />
  );
};

export default ReviewModeration;