import React from 'react';
import { Navigate } from 'react-router-dom';

const PrivateRoute = ({ children, role, requiredRole }) => {
  if (!role) {
    return <Navigate to="/auth" />;
  }
  
  if (requiredRole && role !== requiredRole) {
    return <Navigate to="/" />;
  }
  
  return children;
};

export default PrivateRoute;