import React from 'react';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  isLoading?: boolean;
  variant?: 'primary' | 'secondary' | 'outline';
  size?: 'sm' | 'md' | 'lg';
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      children,
      isLoading = false,
      variant = 'primary',
      size = 'md',
      className = '',
      disabled,
      ...props
    },
    ref
  ) => {
    const baseClasses =
      'font-medium rounded-xl transition-all duration-200 ease-in-out flex items-center justify-center space-x-2 focus:outline-none focus:ring-2 focus:ring-offset-2';

    const variantClasses = {
      primary:
        'bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white shadow-lg hover:shadow-xl focus:ring-blue-500 transform hover:scale-[1.02] active:scale-[0.98]',
      secondary:
        'bg-gray-100 hover:bg-gray-200 text-gray-800 focus:ring-gray-500',
      outline:
        'border-2 border-gray-300 hover:border-gray-400 text-gray-700 bg-white hover:bg-gray-50 focus:ring-gray-500',
    };

    const sizeClasses = {
      sm: 'px-4 py-2 text-sm',
      md: 'px-6 py-3 text-base',
      lg: 'px-8 py-4 text-lg',
    };

    const isDisabled = disabled || isLoading;

    return (
      <button
        ref={ref}
        {...props}
        disabled={isDisabled}
        className={`
          ${baseClasses}
          ${variantClasses[variant]}
          ${sizeClasses[size]}
          ${isDisabled ? 'opacity-50 cursor-not-allowed' : ''}
          ${className}
        `}
      >
        {isLoading && <Loader2 className="w-4 h-4 animate-spin" />}
        <span>{children}</span>
      </button>
    );
  }
);

Button.displayName = 'Button';

export default Button;