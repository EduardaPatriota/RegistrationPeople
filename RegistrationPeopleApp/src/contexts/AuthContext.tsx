import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { API_BASE_URL, apiPost, apiGet, getJtiFromToken } from '../utils/api'; // Importa utilitários

interface User {
  id: string;
  name: string;
  email: string;
  cpf?: string;
  gender?: string;
  birthDate?: string;
  birthplace?: string;
  nationality?: string;
  address?: string;
}

interface RegisterData {
  name: string;
  gender: string;
  email: string;
  birthDate: string; 
  birthplace: string;
  nationality: string;
  cpf: string;
  password: string;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (userData: RegisterData) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const savedToken = localStorage.getItem('authToken');
    if (savedToken) {
      setToken(savedToken);
      fetchUserData(savedToken);
    } else {
      setIsLoading(false);
    }
  }, []);
  

  const fetchUserData = async (authToken: string) => {
    setIsLoading(true);
    try {
      const userId = getJtiFromToken(authToken.replace('Bearer ', ''));
      if (!userId) throw new Error('ID do usuário não encontrado no token');

      // Verifica se o ID do usuário é o do administrador
      if (userId === '00000000-0000-0000-0000-000000000000') {
        setUser({
          id: userId,
          name: 'Administrador',
          email: 'admin@admin.com',
          cpf: '',
          gender: '',
          birthDate: '',
          birthplace: '',
          nationality: '',
          address: '',
        });
        setIsLoading(false);
        return;
      }

      const response = await apiGet(`/v1/Person/${userId}`);
      setUser(response.data);
      console.log('Dados do usuário:', response.data);
    } catch (error) {
      console.error('Erro ao buscar dados do usuário:', error);
      localStorage.removeItem('authToken');
      setToken(null);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

    const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const data = await apiPost('/auth/login', { email, password });
      const authToken = data.data; 
      localStorage.setItem('authToken', authToken);
      setToken(authToken);
      await fetchUserData(authToken); 
    } catch (error) {
      throw error;
    } finally {
      setIsLoading(false);
    }
  };


  const register = async (userData: RegisterData) => {
    setIsLoading(true);
    try {
      const data = await apiPost('/auth/register', userData);
      const { token: authToken, user: newUser } = data;
      localStorage.setItem('authToken', authToken);
      setToken(authToken);
      setUser(newUser);
    } catch (error) {
      console.error('Erro no cadastro:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    setToken(null);
    setUser(null);
  };

  const value = {
    user,
    token,
    login,
    register,
    logout,
    isLoading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};