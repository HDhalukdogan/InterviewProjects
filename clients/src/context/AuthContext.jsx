import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { identityApi } from '../lib/agentIdentity';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Check active cookie session by requesting manage/info
  const checkAuth = useCallback(async () => {
    try {
      const info = await identityApi.getManageInfo();
      if (info && info.email) {
        setUser({
          email: info.email,
          isEmailConfirmed: info.isEmailConfirmed,
        });
      } else {
        setUser(null);
      }
    } catch {
      // 401 or error means no active cookie session
      setUser(null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  const login = async (email, password) => {
    // 1. Authenticate and receive auth cookie from backend
    await identityApi.login({ email, password });

    // 2. Query user profile using the newly set cookie
    try {
      const info = await identityApi.getManageInfo();
      const currentUser = {
        email: info.email || email,
        isEmailConfirmed: info.isEmailConfirmed || false,
      };
      setUser(currentUser);
      return currentUser;
    } catch {
      const fallbackUser = { email, isEmailConfirmed: false };
      setUser(fallbackUser);
      return fallbackUser;
    }
  };

  const register = async (email, password) => {
    return await identityApi.register({ email, password });
  };

  const logout = async () => {
    try {
      await identityApi.logout();
    } finally {
      setUser(null);
    }
  };

  const value = {
    user,
    isAuthenticated: !!user,
    loading,
    login,
    register,
    logout,
    refreshUser: checkAuth,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;
