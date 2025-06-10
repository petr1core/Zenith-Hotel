import React, { useState, useEffect } from 'react';
import { List, Rate, Typography, Spin, Empty } from 'antd';
import { reviewService } from '../services/reviewService';
import ReviewCard from './ReviewCard';

const { Title, Text } = Typography;

const ReviewList = ({ roomId }) => {
    const [loading, setLoading] = useState(true);
    const [reviews, setReviews] = useState([]);
    const [rating, setRating] = useState({ averageRating: 0, totalReviews: 0 });
    const [pagination, setPagination] = useState({
        current: 1,
        pageSize: 10,
        total: 0
    });

    const loadReviews = async (page = 1) => {
        try {
            setLoading(true);
            const [reviewsResponse, ratingResponse] = await Promise.all([
                reviewService.getRoomReviews(roomId, page, pagination.pageSize),
                reviewService.getRoomRating(roomId)
            ]);

            console.log('Reviews response:', reviewsResponse.data);
            console.log('Rating response:', ratingResponse.data);

            setReviews(reviewsResponse.data.items);
            const ratingData = ratingResponse.data || {};
            setRating({
                averageRating: ratingData.averageRating || 0,
                totalReviews: ratingData.totalReviews || 0,
            });
            setPagination({
                ...pagination,
                current: page,
                total: reviewsResponse.data.totalCount
            });
        } catch (error) {
            console.error('Ошибка при загрузке отзывов:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadReviews();
    }, [roomId]);

    const handlePageChange = (page) => {
        loadReviews(page);
    };

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <div className="reviews-container">
            <div style={{ marginBottom: 24, textAlign: 'center' }}>
                <Title level={4}>Отзывы гостей</Title>
                {rating.totalReviews > 0 && (
                    <div>
                        <Rate
                            disabled
                            value={rating.averageRating}
                            style={{ fontSize: 24 }}
                        />
                        <Text strong style={{ fontSize: 18, marginLeft: 8 }}>
                            {rating.averageRating.toFixed(1)}
                        </Text>
                        <Text type="secondary" style={{ fontSize: 14, marginLeft: 8 }}>
                            ({rating.totalReviews} {rating.totalReviews === 1 ? 'отзыв' :
                                rating.totalReviews < 5 ? 'отзыва' : 'отзывов'})
                        </Text>
                    </div>
                )}
            </div>

            {reviews.length > 0 ? (
                <List
                    dataSource={reviews}
                    renderItem={(review) => (
                        <List.Item>
                            <ReviewCard review={review} />
                        </List.Item>
                    )}
                    pagination={{
                        ...pagination,
                        onChange: handlePageChange
                    }}
                />
            ) : (
                <Empty
                    description="Пока нет отзывов"
                    style={{ margin: '40px 0' }}
                />
            )}
        </div>
    );
};

export default ReviewList; 