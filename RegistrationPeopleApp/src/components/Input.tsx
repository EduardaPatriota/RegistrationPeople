
import React from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  icon?: React.ReactNode;
  rightIcon?: React.ReactNode;
}

const Input = ({ icon, rightIcon, className = '', ...props }: InputProps) => {
  return (
    <div className="relative">
      {icon && (
        <div className="absolute left-3 top-1/2 transform -translate-y-1/2 z-10">
          {icon}
        </div>
      )}
      <input
        {...props}
        className={`
          w-full px-4 py-3 bg-gray-50 border border-gray-200 rounded-xl
          focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
          transition-all duration-200 ease-in-out
          hover:border-gray-300
          ${icon ? 'pl-12' : ''}
          ${rightIcon ? 'pr-12' : ''}
          ${className}
        `}
      />
      {rightIcon && (
        <div className="absolute right-3 top-1/2 transform -translate-y-1/2">
          {rightIcon}
        </div>
      )}
    </div>
  );
};

export default Input;
