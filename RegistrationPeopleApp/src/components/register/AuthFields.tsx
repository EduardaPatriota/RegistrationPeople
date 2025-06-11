
import React from 'react';
import { Mail, Lock } from 'lucide-react';
import Input from '@/components/Input';

interface AuthFieldsProps {
  formData: {
    email: string;
    senha: string;
    confirmarSenha: string;
  };
  onInputChange: (field: string, value: string) => void;
}

const AuthFields: React.FC<AuthFieldsProps> = ({ formData, onInputChange }) => {
  return (
    <>
      <div>
        <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
          E-mail
        </label>
        <Input
          id="email"
          type="email"
          placeholder="Digite seu e-mail"
          value={formData.email}
          onChange={(e) => onInputChange('email', e.target.value)}
          icon={<Mail className="w-5 h-5 text-gray-400" />}
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <label htmlFor="senha" className="block text-sm font-medium text-gray-700 mb-2">
            Senha *
          </label>
          <Input
            id="senha"
            type="password"
            placeholder="Digite sua senha"
            value={formData.senha}
            onChange={(e) => onInputChange('senha', e.target.value)}
            icon={<Lock className="w-5 h-5 text-gray-400" />}
            required
          />
        </div>

        <div>
          <label htmlFor="confirmarSenha" className="block text-sm font-medium text-gray-700 mb-2">
            Confirmar Senha *
          </label>
          <Input
            id="confirmarSenha"
            type="password"
            placeholder="Confirme sua senha"
            value={formData.confirmarSenha}
            onChange={(e) => onInputChange('confirmarSenha', e.target.value)}
            icon={<Lock className="w-5 h-5 text-gray-400" />}
            required
          />
        </div>
      </div>
    </>
  );
};

export default AuthFields;
