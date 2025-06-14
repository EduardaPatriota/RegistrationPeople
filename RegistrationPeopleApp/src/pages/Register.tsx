
import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus } from 'lucide-react';
import Button from '@/components/Button';
import { toast } from '@/components/ui/use-toast';
import { useAuth } from '../contexts/AuthContext';
import { useRegisterForm } from '../hooks/useRegisterForm';
import PersonalInfoFields from '../components/register/PersonalInfoFields';
import AuthFields from '../components/register/AuthFields';
import LocationFields from '../components/register/LocationFields';
import {
  validateEmail,
  validateCPF,
  validateAge,
  validatePassword,
  validatePasswordMatch,
  validateRequired
} from '../utils/validation';

const Register = () => {
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const { register } = useAuth();
  const { formData, handleInputChange } = useRegisterForm();

  const validateForm = () => {
    if (!validateRequired(formData.nome)) {
      toast({
        title: "Erro de validação",
        description: "Nome é obrigatório",
        variant: "destructive",
      });
      return false;
    }

    if (!validateRequired(formData.senha)) {
      toast({
        title: "Erro de validação",
        description: "Senha é obrigatória",
        variant: "destructive",
      });
      return false;
    }

    if (!validatePassword(formData.senha)) {
      toast({
        title: "Erro de validação",
        description: "Senha deve ter pelo menos 6 caracteres",
        variant: "destructive",
      });
      return false;
    }

    if (!validatePasswordMatch(formData.senha, formData.confirmarSenha)) {
      toast({
        title: "Erro de validação",
        description: "As senhas não coincidem",
        variant: "destructive",
      });
      return false;
    }

    if (formData.email && !validateEmail(formData.email)) {
      toast({
        title: "Erro de validação",
        description: "E-mail inválido",
        variant: "destructive",
      });
      return false;
    }

    if (!formData.dataNascimento) {
      toast({
        title: "Erro de validação",
        description: "Data de nascimento é obrigatória",
        variant: "destructive",
      });
      return false;
    }

    if (!validateAge(formData.dataNascimento)) {
      toast({
        title: "Erro de validação",
        description: "Data de nascimento inválida",
        variant: "destructive",
      });
      return false;
    }

    if (!validateCPF(formData.cpf)) {
      toast({
        title: "Erro de validação",
        description: "CPF deve ter 11 dígitos",
        variant: "destructive",
      });
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    if (!validateForm()) {
      setIsLoading(false);
      return;
    }

    try {
      await register({
        Name: formData.nome,
        Email: formData.email,
        Password: formData.senha,
        BirthDate: formData.dataNascimento,
        Gender: formData.sexo,
        Birthplace: formData.naturalidade,
        Nationality: formData.nacionalidade,
        Cpf: formData.cpf.replace(/\D/g, ''),
      });

      toast({
        title: "Cadastro efetuado  com sucesso!",
        description: "Seu cadastro foi realizado com sucesso. Agora você já pode acessar a plataforma.",

      });
      navigate('/dashboard');
    } catch (error) {
      toast({
        title: "Erro no cadastro",
        description: error instanceof Error ? error.message : "Erro interno do servidor",
        variant: "destructive",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-gradient-to-br from-blue-400/20 via-transparent to-purple-400/20"></div>
      <div className="absolute top-0 left-0 w-72 h-72 bg-blue-300 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob"></div>
      <div className="absolute top-0 right-0 w-72 h-72 bg-purple-300 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-2000"></div>
      <div className="absolute -bottom-8 left-20 w-72 h-72 bg-pink-300 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-4000"></div>
      
      <div className="relative z-10 w-full max-w-2xl">
        <div className="bg-white/90 backdrop-blur-sm rounded-2xl shadow-xl border border-white/20 p-8">
          <div className="text-center mb-8">
            <div className="w-16 h-16 bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl flex items-center justify-center mx-auto mb-4">
              <UserPlus className="w-8 h-8 text-white" />
            </div>
            <h2 className="text-3xl font-bold text-gray-800 mb-2">Criar Conta</h2>
            <p className="text-gray-600">Preencha os dados para se cadastrar</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            <PersonalInfoFields formData={formData} onInputChange={handleInputChange} />
            <AuthFields formData={formData} onInputChange={handleInputChange} />
            <LocationFields formData={formData} onInputChange={handleInputChange} />

            <Button type="submit" className="w-full" isLoading={isLoading}>
              <UserPlus className="w-4 h-4 mr-2" />
              Cadastrar
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-gray-600">
              Já tem uma conta?{' '}
              <Link to="/" className="text-blue-600 hover:text-blue-700 font-medium">
                Faça login
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;
