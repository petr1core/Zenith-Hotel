import React from 'react';
import { Link } from 'react-router-dom';

const AdminPanel = () => (
    <div>
      <h1>Admin Dashboard</h1>
      <div className="admin-grid">
        <Link to="/admin/rooms" className="admin-card">
          Manage Rooms
        </Link>
        <Link to="/admin/bookings" className="admin-card">
          View All Bookings
        </Link>
        <Link to="/admin/users" className="admin-card">
          Manage Users
        </Link>
      </div>
    </div>
  );
export default AdminPanel;