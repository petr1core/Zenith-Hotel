import api from './apiService';

export const reviewService = {
    // Получение всех отзывов для номера
    async getRoomReviews(roomId, page = 1, pageSize = 10) {
        return await api.get(`/Review/room/${roomId}?page=${page}&pageSize=${pageSize}`);
    },

    // Создание нового отзыва
    async createReview(reviewData) {
        return await api.post('/Review', reviewData);
    },

    // Получение среднего рейтинга номера
    async getRoomRating(roomId) {
        return await api.get(`/Review/room/${roomId}/rating`);
    },

    // Удаление отзыва (для администратора)
    async deleteReview(reviewId) {
        return await api.delete(`/Review/${reviewId}`);
    },

    // Модерация отзыва (для администратора)
    async moderateReview(reviewId, moderationData) {
        return await api.put(`/Review/${reviewId}/moderate`, moderationData);
    }
};

export default reviewService; 