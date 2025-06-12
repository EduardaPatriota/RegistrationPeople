

export const API_BASE_URL = 'https://localhost:7247/api'; 

export const apiRequest = async (endpoint: string, options: RequestInit = {}) => {
  const token = localStorage.getItem('authToken');
  
  const config: RequestInit = {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  };

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
    
    if (response.status === 401) {
      localStorage.removeItem('authToken');
      window.location.href = '/';
      return;
    }

    if (response.status === 204) {
      return null;
    }

    if (!response.ok) {
      throw new Error(`Erro ${response.status}: ${response.statusText}`);
    }


    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      return await response.json();
    }
    return null;
  } catch (error) {
    console.error('Erro na requisição:', error);
    throw error;
  }
};

 export function getJtiFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.jti || null;
    } catch {
      return null;
    }
  }

export const apiGet = (endpoint: string) => apiRequest(endpoint);

export const apiPost = (endpoint: string, data: any) => 
  apiRequest(endpoint, {
    method: 'POST',
    body: JSON.stringify(data),
  });

export const apiPut = (endpoint: string, data: any) => 
  apiRequest(endpoint, {
    method: 'PUT',
    body: JSON.stringify(data),
  });

export const apiDelete = (endpoint: string) => 
  apiRequest(endpoint, {
    method: 'DELETE',
  });
