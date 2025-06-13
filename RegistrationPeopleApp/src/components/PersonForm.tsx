import React, { useState, useEffect } from 'react';
import { User, Mail, FileText, MapPin, Globe, Calendar, Home } from 'lucide-react';
import { formatCPF } from '@/utils/formatters';
import Input from './Input';
import Button from './Button';

interface Person {
  id?: string;
  name: string;
  gender: string;
  email: string;
  birthDate: string;
  birthplace: string;
  nationality: string;
  cpf: string;
  address?: string;
}

interface PersonFormProps {
  person?: Person | null;
  onSubmit: (person: Omit<Person, 'id'>) => void;
  onCancel: () => void;
}

const PersonForm = ({ person, onSubmit, onCancel }: PersonFormProps) => {
  const [formData, setFormData] = useState<Omit<Person, 'id'>>({
    name: '',
    gender: '',
    email: '',
    birthDate: '',
    birthplace: '',
    nationality: '',
    cpf: '',
    address: '',
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (person) {  
      setFormData({
        name: person.name || '',
        gender: person.gender || '',
        email: person.email || '',
        birthDate: person.birthDate ? person.birthDate.slice(0, 10) : '',
        birthplace: person.birthplace || '',
        nationality: person.nationality || '',
        cpf: person.cpf || '',
        address: person.address || '',
      });
      
    } else {
      setFormData({
        name: '',
        gender: '',
        email: '',
        birthDate: '',
        birthplace: '',
        nationality: '',
        cpf: '',
        address: '',
      });
    }
  }, [person]);

  const handleChange = (field: keyof typeof formData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    const cleanCPF = formData.cpf.replace(/\D/g, '');
    const dataToSend = { ...formData, cpf: cleanCPF };

    if (formData.address && formData.address.trim() !== '') {
      await onSubmit({ ...dataToSend, apiVersion: 'v2' } as any);
    } else {
      await onSubmit(dataToSend);
    }

    setIsLoading(false);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        type="text"
        placeholder="Nome completo"
        value={formData.name}
        onChange={e => handleChange('name', e.target.value)}
        icon={<User className="w-5 h-5 text-gray-400" />}
        required
      />
      <div>
        <div className="relative">
          <User className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 transform -translate-y-1/2 pointer-events-none" />
          <select
            id="gender"
            value={formData.gender}
            onChange={e => handleChange('gender', e.target.value)}
            className="w-full pl-12 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-in-out hover:border-gray-300 appearance-none"
            required
          >
            {!person && <option value="">Selecione o gênero</option>}
            <option value="masculino">Masculino</option>
            <option value="feminino">Feminino</option>
            <option value="outro">Outro</option>
            <option value="doNotSpecify">Prefiro não informar</option>
          </select>
        </div>
      </div>
      <Input
        type="email"
        placeholder="Email"
        value={formData.email}
        onChange={e => handleChange('email', e.target.value)}
        icon={<Mail className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="date"
        placeholder="Data de nascimento"
        value={formData.birthDate}
        onChange={e => handleChange('birthDate', e.target.value)}
        icon={<Calendar className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="Naturalidade"
        value={formData.birthplace}
        onChange={e => handleChange('birthplace', e.target.value)}
        icon={<MapPin className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="Nacionalidade"
        value={formData.nationality}
        onChange={e => handleChange('nationality', e.target.value)}
        icon={<Globe className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="CPF"
        value={formData.cpf}
        onChange={e => handleChange('cpf', formatCPF(e.target.value))}
        icon={<FileText className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="Endereço"
        value={formData.address}
        onChange={e => handleChange('address', e.target.value)}
        icon={<Home className="w-5 h-5 text-gray-400" />}
      />

      <div className="flex gap-3 pt-4">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          className="flex-1"
        >
          Cancelar
        </Button>
        <Button
          type="submit"
          isLoading={isLoading}
          className="flex-1"
        >
          {isLoading ? 'Salvando...' : (person ? 'Atualizar' : 'Cadastrar')}
        </Button>
      </div>
    </form>
  );
};

export default PersonForm;