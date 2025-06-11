
export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

export const validateCPF = (cpf: string): boolean => {
  const cleanCPF = cpf.replace(/\D/g, '');
  return cleanCPF.length === 11;
};

export const validateAge = (dataNascimento: string): boolean => {
  const today = new Date();
  const birthDate = new Date(dataNascimento);
  const age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    return age - 1 >= 0;
  }
  return age >= 0;
};

export const validatePassword = (password: string): boolean => {
  return password.length >= 6;
};

export const validatePasswordMatch = (password: string, confirmPassword: string): boolean => {
  return password === confirmPassword;
};

export const validateRequired = (value: string): boolean => {
  return value.trim().length > 0;
};
