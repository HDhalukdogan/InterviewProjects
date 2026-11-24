import axios from 'axios';

const BASE_URL = 'http://localhost:5118/api/';

const agentIdentity = axios.create({
  baseURL: BASE_URL,
  withCredentials: true, // Enables browser to automatically send and receive cookies (HttpOnly auth cookie)
  headers: {
    'Content-Type': 'application/json',
  },
});

export const identityApi = {
  // Login using ASP.NET Core Identity Cookie authentication
  login: async (credentials) => {
    const response = await agentIdentity.post('login?useCookies=true', {
      email: credentials.email,
      password: credentials.password,
    });
    return response.data;
  },

  // Register a new user
  register: async (credentials) => {
    const response = await agentIdentity.post('register', {
      email: credentials.email,
      password: credentials.password,
    });
    return response.data;
  },

  // Logout: signs out from backend and clears auth cookie
  logout: async () => {
    try {
      const response = await agentIdentity.post('logout');
      return response.data;
    } catch (err) {
      console.warn('Logout endpoint error:', err?.message || err);
    }
  },

  // Retrieve current user profile / verify cookie session validity
  getManageInfo: async () => {
    const response = await agentIdentity.get('manage/info');
    return response.data;
  },

  // Get WeatherForecast data from IdentityApp
  getWeatherForecast: async () => {
    const response = await agentIdentity.get('weatherforecast');
    return response.data;
  },
};

export default agentIdentity;
