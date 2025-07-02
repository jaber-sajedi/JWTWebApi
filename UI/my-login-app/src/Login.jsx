import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Login.css';

export default function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await fetch('https://localhost:7265/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      });

      if (res.ok) {
        const data = await res.json();
        localStorage.setItem('token', data.token);
        setMessage('ورود موفق! در حال انتقال...');
        navigate('/dashboard');
      } else {
        setMessage('نام کاربری یا رمز عبور اشتباه است.');
      }
    } catch (error) {
      setMessage('خطا در اتصال به سرور.');
      console.error(error);
    }
  };

  return (
    <div className="login-container">
      <h2>ورود به سیستم</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>نام کاربری</label>
          <input
            type="text"
            value={username}
            onChange={e => setUsername(e.target.value)}
            placeholder="مثلاً: admin"
            required
          />
        </div>
        <div className="form-group">
          <label>رمز عبور</label>
          <input
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            placeholder="مثلاً: 1234"
            required
          />
        </div>
        <button type="submit">ورود</button>
      </form>
      <div className="message">{message}</div>
    </div>
  );
}
