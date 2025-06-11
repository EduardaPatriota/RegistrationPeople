
import React from 'react';
import { MapPin, Globe } from 'lucide-react';
import Input from '@/components/Input';

interface LocationFieldsProps {
  formData: {
    naturalidade: string;
    nacionalidade: string;
  };
  onInputChange: (field: string, value: string) => void;
}

const LocationFields: React.FC<LocationFieldsProps> = ({ formData, onInputChange }) => {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
      <div>
        <label htmlFor="naturalidade" className="block text-sm font-medium text-gray-700 mb-2">
          Naturalidade
        </label>
        <Input
          id="naturalidade"
          type="text"
          placeholder="Cidade onde nasceu"
          value={formData.naturalidade}
          onChange={(e) => onInputChange('naturalidade', e.target.value)}
          icon={<MapPin className="w-5 h-5 text-gray-400" />}
        />
      </div>

      <div>
        <label htmlFor="nacionalidade" className="block text-sm font-medium text-gray-700 mb-2">
          Nacionalidade
        </label>
        <Input
          id="nacionalidade"
          type="text"
          placeholder="País de origem"
          value={formData.nacionalidade}
          onChange={(e) => onInputChange('nacionalidade', e.target.value)}
          icon={<Globe className="w-5 h-5 text-gray-400" />}
        />
      </div>
    </div>
  );
};

export default LocationFields;
