// src/PrivateRoute.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';

export default function PrivateRoute({ children }) {
  const token = localStorage.getItem('token');

  // اگر توکن موجود نباشد، به صفحه ورود برگرد
  if (!token) {
    return <Navigate to="/login" />;
  }

  return children;
}
