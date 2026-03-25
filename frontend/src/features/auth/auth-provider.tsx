import { createContext, useContext, useState, useCallback, useMemo } from 'react';
import type { ReactNode } from 'react';
import { api } from '@/lib/api';

interface AuthContextType {
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, firstName: string, lastName: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

function isTokenValid(token: string | null): boolean {
  if (!token) return false;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const exp = payload.exp as number;
    return Date.now() < exp * 1000;
  } catch {
    return false;
  }
}

function getStoredToken(): string | null {
  const token = localStorage.getItem('token');
  if (token && !isTokenValid(token)) {
    localStorage.removeItem('token');
    return null;
  }
  return token;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(getStoredToken);

  const login = useCallback(async (email: string, password: string) => {
    const { token: newToken } = await api.post<{ token: string }>('/users/login', { email, password });
    localStorage.setItem('token', newToken);
    setToken(newToken);
  }, []);

  const register = useCallback(async (email: string, firstName: string, lastName: string, password: string) => {
    await api.post<{ userId: string }>('/users/register', { email, firstName, lastName, password });
    await login(email, password);
  }, [login]);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    setToken(null);
  }, []);

  const isAuthenticated = useMemo(() => isTokenValid(token), [token]);

  return (
    <AuthContext value={{ token, isAuthenticated, login, register, logout }}>
      {children}
    </AuthContext>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
