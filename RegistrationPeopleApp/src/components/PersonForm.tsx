import React, { useState, useEffect } from 'react';
import { User, Mail, FileText, MapPin, Globe, Calendar, Home } from 'lucide-react';
import { formatCPF } from '@/utils/formatters';
import Input from './Input';
import Button from './Button';
import { toast } from '@/components/ui/use-toast';

interface Person {
  Id?: string;
  Name: string;
  Gender: string;
  Email: string;
  BirthDate: string;
  Birthplace: string;
  Nationality: string;
  Cpf: string;
  Address?: string;
}

interface PersonFormProps {
  person?: Person | null;
  onSubmit: (person: Omit<Person, 'id'>) => void;
  onCancel: () => void;
}

const PersonForm = ({ person, onSubmit, onCancel }: PersonFormProps) => {
  const [formData, setFormData] = useState<Omit<Person, 'id'>>({
    Name: '',
    Gender: '',
    Email: '',
    BirthDate: '',
    Birthplace: '',
    Nationality: '',
    Cpf: '',
    Address: '',

  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (person) {  
      setFormData({
        Name: person.Name || '',
        Gender: person.Gender || '',
        Email: person.Email || '',
        BirthDate: person.BirthDate ? person.BirthDate.slice(0, 10) : '',
        Birthplace: person.Birthplace || '',
        Nationality: person.Nationality || '',
        Cpf: person.Cpf || '',
        Address: person.Address || '',
      });
      
    } else {
      setFormData({
        Name: '',
        Gender: '',
        Email: '',
        BirthDate: '',
        Birthplace: '',
        Nationality: '',
        Cpf: '',
        Address: '',
      });
    }
  }, [person]);

  const handleChange = (field: keyof typeof formData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const cleanCPF = formData.Cpf.replace(/\D/g, '');
      const dataToSend = { ...formData, Cpf: cleanCPF };

      if (formData.Address && formData.Address.trim() !== '') {
        await onSubmit({ ...dataToSend, apiVersion: 'v2' } as any);
      } else {
        await onSubmit(dataToSend);
      }


    } catch (error: any) {
      
      const errors = error.data?.errors ?? error;
     
      if (errors) {
        let messages = "";

        if (errors instanceof Error) {
          messages = errors.message;   
        } else if (typeof errors === 'object' && errors !== null) {
          messages = Object.values(errors).flat().join('\n');
        } else {
          messages = String(errors);
        }

        toast({
          title: "Erro de validação",
          description: messages,
          variant: "destructive",
        });
      }
      
    } finally {
      setIsLoading(false);
    }
  };



  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Input
        type="text"
        placeholder="Nome completo"
        value={formData.Name}
        onChange={e => handleChange('Name', e.target.value)}
        icon={<User className="w-5 h-5 text-gray-400" />}
        required
      />
      <div>
        <div className="relative">
          <User className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 transform -translate-y-1/2 pointer-events-none" />
          <select
            id="gender"
            value={formData.Gender}
            onChange={e => handleChange('Gender', e.target.value)}
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
        value={formData.Email}
        onChange={e => handleChange('Email', e.target.value)}
        icon={<Mail className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="date"
        placeholder="Data de nascimento"
        value={formData.BirthDate}
        onChange={e => handleChange('BirthDate', e.target.value)}
        icon={<Calendar className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="Naturalidade"
        value={formData.Birthplace}
        onChange={e => handleChange('Birthplace', e.target.value)}
        icon={<MapPin className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="Nacionalidade"
        value={formData.Nationality}
        onChange={e => handleChange('Nationality', e.target.value)}
        icon={<Globe className="w-5 h-5 text-gray-400" />}
        required
      />
      <Input
        type="text"
        placeholder="CPF"
        value={formData.Cpf}
        onChange={e => handleChange('Cpf', formatCPF(e.target.value))}
        icon={<FileText className="w-5 h-5 text-gray-400" />}
        
        required
      />
      <Input
        type="text"
        placeholder="Endereço"
        value={formData.Address}
        onChange={e => handleChange('Address', e.target.value)}
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