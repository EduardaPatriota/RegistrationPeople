//const API_BASE_URL = 'https://localhost:7247/api';
const API_BASE_URL = "https://registrationpeopleapi-bsdfd3gygndzeaeq.brazilsouth-01.azurewebsites.net/api";

type ApiMethod = 'GET' | 'POST' | 'PUT' | 'DELETE';

interface ApiOptions extends RequestInit {
  body?: any;
}

const getAuthHeaders = () => {
  const token = localStorage.getItem('authToken');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

const handleResponse = async (response: Response) => {
  if (response.status === 401) {
    localStorage.removeItem('authToken');
    alert('Sua sessão expirou. Faça login novamente.');
    window.location.href = '/';
    return;
  }
  if (response.status === 404) {
    throw new Error('Recurso não encontrado.');
  }

  if (response.status === 204) return null;

  if (response.status === 400) {
    const errorData = await response.json();
    const error = new Error(errorData.title || errorData['Errors']);
    (error as any).data = errorData; 
    throw error;
  }

  if (!response.ok) {
    const errorData = await response.json();
    const customError = new Error(errorData.title || 'Erro de requisição');
    (customError as any).data = errorData;
    
    throw customError;
  }
  

  const contentType = response.headers.get('content-type');
  if (contentType?.includes('application/json')) {
    return response.json();
  }

  return response.json();
};

export const apiRequest = async (
  endpoint: string,
  method: ApiMethod = 'GET',
  data?: any,
  options: ApiOptions = {}
) => {
  const config: RequestInit = {
    method,
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...getAuthHeaders(),
      ...options.headers,
    },
    ...(data && { body: JSON.stringify(data) }),
  };

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
    return await handleResponse(response);
  } catch (error) {
    throw error;
  }
};

export const apiGet = (endpoint: string, options?: ApiOptions) =>
  apiRequest(endpoint, 'GET', undefined, options);

export const apiPost = (endpoint: string, data: any, options?: ApiOptions) =>
  apiRequest(endpoint, 'POST', data, options);

export const apiPut = (endpoint: string, data: any, options?: ApiOptions) =>
  apiRequest(endpoint, 'PUT', data, options);

export const apiDelete = (endpoint: string, options?: ApiOptions) =>
  apiRequest(endpoint, 'DELETE', undefined, options);

export function getJtiFromToken(token: string): string | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.jti || null;
  } catch {
    return null;
  }
}