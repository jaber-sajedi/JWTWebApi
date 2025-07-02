import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Dashboard.css';

export default function Dashboard() {
  const token = localStorage.getItem('token');
  const navigate = useNavigate();

  return (
    <div className="dashboard-container">
      <h2>صفحه داشبورد</h2>

      {token ? (
        <div className="token-box">
          <strong>توکن شما:</strong>
          <pre>{token}</pre>
          <button
            onClick={() => {
              navigator.clipboard.writeText(token);
              alert('توکن در کلیپ‌بورد کپی شد.');
            }}
          >
            کپی توکن
          </button>
        </div>
      ) : (
        <p className="no-token-text">برای مشاهده‌ی این صفحه ابتدا وارد شوید.</p>
      )}

      <button
        className="back-button"
        onClick={() => {
          navigate('/');
        }}
      >
        بازگشت به فرم ورود
      </button>
    </div>
  );
}
