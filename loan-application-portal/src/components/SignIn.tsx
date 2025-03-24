import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './SignIn.css';
import logo from '../assets/logo.png';

interface SignInProps {
    onLogin: () => void;
}

const SignIn: React.FC<SignInProps> = ({onLogin}) => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Simple authenticate logic
    if (username === 'admin' && password === 'password') {
        // Call the onLogin callback to update authentication status
        onLogin();
        // Navigate to the dashboard
        navigate('/dashboard');
      } else {
        // Show an error message if credentials are incorrect
        setError('Invalid username or password');
      }
  };

  return (
    <div className="signin-container">
      <div className="logo">
        <img src={logo} alt="Company Logo" />
        <h1>VinNance Credit Loan Management</h1>
      </div>
      <div className="signin-form">
        <h2>Sign In</h2>
        <form onSubmit={handleSubmit}>
          <div>
            <label>Username:</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>
          <div>
            <label>Password:</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          {error && <p className="error-message">{error}</p>}
          <button type="submit">Sign In</button>
        </form>
      </div>
    </div>
  );
};

export default SignIn;