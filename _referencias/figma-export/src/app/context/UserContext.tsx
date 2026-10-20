import { createContext, useContext, useState, ReactNode } from 'react';

export interface UserPreferences {
  nombre: string;
  restricciones: string[];
  alergias: string[];
  gustos: string[];
  nivelCocina: 'principiante' | 'intermedio' | 'avanzado';
}

interface UserContextType {
  preferences: UserPreferences | null;
  setPreferences: (prefs: UserPreferences) => void;
  isOnboarded: boolean;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export function UserProvider({ children }: { children: ReactNode }) {
  const [preferences, setPreferences] = useState<UserPreferences | null>(null);
  const [isOnboarded, setIsOnboarded] = useState(false);

  const handleSetPreferences = (prefs: UserPreferences) => {
    setPreferences(prefs);
    setIsOnboarded(true);
  };

  return (
    <UserContext.Provider value={{ preferences, setPreferences: handleSetPreferences, isOnboarded }}>
      {children}
    </UserContext.Provider>
  );
}

export function useUser() {
  const context = useContext(UserContext);
  if (context === undefined) {
    throw new Error('useUser debe ser usado dentro de un UserProvider');
  }
  return context;
}
