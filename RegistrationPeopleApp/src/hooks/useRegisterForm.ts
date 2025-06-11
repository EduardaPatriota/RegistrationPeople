
import { useState } from 'react';
import { formatCPF } from '@/utils/formatters';

interface FormData {
  nome: string;
  sexo: string;
  email: string;
  senha: string;
  confirmarSenha: string;
  dataNascimento: string;
  naturalidade: string;
  nacionalidade: string;
  cpf: string;
}

export const useRegisterForm = () => {
  const [formData, setFormData] = useState<FormData>({
    nome: '',
    sexo: '',
    email: '',
    senha: '',
    confirmarSenha: '',
    dataNascimento: '',
    naturalidade: '',
    nacionalidade: '',
    cpf: ''
  });

  const handleInputChange = (field: keyof FormData, value: string) => {
    if (field === 'cpf') {
      value = formatCPF(value);
    }
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const resetForm = () => {
    setFormData({
      nome: '',
      sexo: '',
      email: '',
      senha: '',
      confirmarSenha: '',
      dataNascimento: '',
      naturalidade: '',
      nacionalidade: '',
      cpf: ''
    });
  };

  return {
    formData,
    handleInputChange,
    resetForm
  };
};
