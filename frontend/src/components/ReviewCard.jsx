import React from 'react';
import { Card, Rate, Typography, Tag } from 'antd';
import { format, parseISO } from 'date-fns';
import { ru } from 'date-fns/locale';

const { Text } = Typography;

const ReviewCard = ({ review }) => {
    const userName = review.user ? `${review.user.firstName} ${review.user.lastName}` : 'Гость';

    const formatDate = (dateString) => {
        if (!dateString) return '';
        // Заменяем пробел на 'T', чтобы создать валидный ISO-формат
        const validDateString = dateString.replace(' ', 'T');
        return format(parseISO(validDateString), 'd MMMM yyyy', { locale: ru });
    };

    return (
        <Card className="review-card" bordered={false} style={{ marginBottom: 16 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
                <div>
                    <Rate disabled value={typeof review.rating === 'number' ? review.rating : 0} style={{ fontSize: 16 }} />
                    <Text style={{ marginLeft: 8 }}>{userName}</Text>
                </div>
                <Text type="secondary">
                    {formatDate(review.createdAt)}
                </Text>
            </div>

            <Text>{review.comment}</Text>

            {review.status === 'Approved' && (
                <div style={{ marginTop: 8 }}>
                    {review.tag ? (
                        <Tag color="blue">{review.tag}</Tag>
                    ) : (
                        <Tag color="green">Одобрен</Tag>
                    )}
                </div>
            )}
        </Card>
    );
};

export default ReviewCard; 