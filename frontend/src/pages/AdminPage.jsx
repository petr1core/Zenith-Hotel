import React from 'react';
import ReviewModeration from '../components/ReviewModeration';
import './AdminPage.css';

const AdminPage = () => {
    return (
        <div className="admin-page-container">
            <h1>Панель администратора</h1>
            <div className="admin-section">
                <h2>Модерация отзывов</h2>
                <ReviewModeration />
            </div>
            {/* Здесь можно будет добавить другие админские компоненты */}
        </div>
    );
};

export default AdminPage; 