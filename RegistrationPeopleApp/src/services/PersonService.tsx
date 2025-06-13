// services/personService.ts
import { apiGet, apiPost, apiPut, apiDelete } from '@/utils/api';

export interface Person {
  Id: string;
  Name: string;
  Email: string;
  Cpf: string;
  Gender: string;
  BirthDate: string;
  Birthplace: string;
  Nationality: string;
  Address: string;
  Password?: string;
  apiVersion?: string;
}

const getPersonEndpoint = (apiVersion?: string, id?: string) => {
  const version = apiVersion === 'v2' ? 'v2' : 'v1';
  let endpoint = `/${version}/Person`;
  if (id) endpoint += `/${id}`;
  return endpoint;
};

export const fetchPeople = async () => {
  return apiGet('/v1/Person');
};

export const fetchPersonById = async (id: string) => {
  return apiGet(`/v1/Person/${id}`);
};

export const addPerson = async (personData: Omit<Person, 'id'>) => {
  const { apiVersion, ...data } = personData;
  return apiPost(getPersonEndpoint(apiVersion), data);
};

export const editPerson = async (id: string, personData: Omit<Person, 'id'>) => {
  const { apiVersion, ...data } = personData;
  return apiPut(getPersonEndpoint(apiVersion, id), data);
};

export const deletePerson = async (id: string) => {
  return apiDelete(`/v1/Person/${id}`);
};
