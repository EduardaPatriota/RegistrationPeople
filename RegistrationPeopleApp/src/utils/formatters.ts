
export const formatCPF = (value: string): string => {
  // Remove tudo que não é dígito
  const digits = value.replace(/\D/g, '');
  
  // Aplica a máscara XXX.XXX.XXX-XX
  if (digits.length <= 3) return digits;
  if (digits.length <= 6) return `${digits.slice(0, 3)}.${digits.slice(3)}`;
  if (digits.length <= 9) return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6)}`;
  return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6, 9)}-${digits.slice(9, 11)}`;
};

export const formatPhone = (value: string): string => {
  // Remove tudo que não é dígito
  const digits = value.replace(/\D/g, '');
  
  // Aplica a máscara (XX) XXXXX-XXXX ou (XX) XXXX-XXXX
  if (digits.length <= 2) return digits;
  if (digits.length <= 7) return `(${digits.slice(0, 2)}) ${digits.slice(2)}`;
  if (digits.length <= 10) return `(${digits.slice(0, 2)}) ${digits.slice(2, 6)}-${digits.slice(6)}`;
  return `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7, 11)}`;
};

export const removeCPFMask = (value: string): string => {
  return value.replace(/\D/g, '');
};

export const removePhoneMask = (value: string): string => {
  return value.replace(/\D/g, '');
};
