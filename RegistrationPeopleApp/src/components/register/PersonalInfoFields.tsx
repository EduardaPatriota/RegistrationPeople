
import React from 'react';
import { User, Calendar, CreditCard } from 'lucide-react';
import Input from '@/components/Input';

interface PersonalInfoFieldsProps {
  formData: {
    nome: string;
    sexo: string;
    dataNascimento: string;
    cpf: string;
  };
  onInputChange: (field: string, value: string) => void;
}

const PersonalInfoFields: React.FC<PersonalInfoFieldsProps> = ({ formData, onInputChange }) => {
  return (
    <>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <label htmlFor="nome" className="block text-sm font-medium text-gray-700 mb-2">
            Nome *
          </label>
          <Input
            id="nome"
            type="text"
            placeholder="Digite seu nome completo"
            value={formData.nome}
            onChange={(e) => onInputChange('nome', e.target.value)}
            icon={<User className="w-5 h-5 text-gray-400" />}
            required
          />
        </div>

        <div>
          <label htmlFor="sexo" className="block text-sm font-medium text-gray-700 mb-2">
            Sexo
          </label>
          <select
            id="sexo"
            value={formData.sexo}
            onChange={(e) => onInputChange('sexo', e.target.value)}
            className="w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-in-out hover:border-gray-300"
          >
            <option value="">Selecione</option>
            <option value="masculino">Masculino</option>
            <option value="feminino">Feminino</option>
            <option value="outro">Outro</option>
            <option value="doNotSpecify">Prefiro não informar</option>
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <label htmlFor="dataNascimento" className="block text-sm font-medium text-gray-700 mb-2">
            Data de Nascimento *
          </label>
          <Input
            id="dataNascimento"
            type="date"
            value={formData.dataNascimento}
            onChange={(e) => onInputChange('dataNascimento', e.target.value)}
            icon={<Calendar className="w-5 h-5 text-gray-400" />}
            required
          />
        </div>

        <div>
          <label htmlFor="cpf" className="block text-sm font-medium text-gray-700 mb-2">
            CPF *
          </label>
          <Input
            id="cpf"
            type="text"
            placeholder="000.000.000-00"
            value={formData.cpf}
            onChange={(e) => onInputChange('cpf', e.target.value)}
            icon={<CreditCard className="w-5 h-5 text-gray-400" />}
            maxLength={14}
            required
          />
        </div>
      </div>
    </>
  );
};

export default PersonalInfoFields;
