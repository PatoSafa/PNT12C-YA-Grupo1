import { Outlet, useNavigate, useLocation } from 'react-router';
import { Home, MapPin, User, Refrigerator } from 'lucide-react';
import { UserProvider } from '../context/UserContext';

export function Layout() {
  const navigate = useNavigate();
  const location = useLocation();

  const isOnboarding = location.pathname === '/';

  return (
    <UserProvider>
      <div className="min-h-screen bg-gradient-to-br from-orange-50 via-amber-50 to-yellow-50">
        <Outlet />

        {!isOnboarding && (
          <nav className="fixed bottom-0 left-0 right-0 bg-white border-t border-orange-200 shadow-lg">
            <div className="max-w-lg mx-auto px-4 py-3 flex justify-around items-center">
              <button
                onClick={() => navigate('/home')}
                className={`flex flex-col items-center gap-1 px-4 py-2 rounded-lg transition-colors ${
                  location.pathname === '/home'
                    ? 'text-orange-600 bg-orange-50'
                    : 'text-gray-600 hover:text-orange-500'
                }`}
              >
                <Home size={24} />
                <span className="text-xs">Inicio</span>
              </button>

              <button
                onClick={() => navigate('/despensa')}
                className={`flex flex-col items-center gap-1 px-4 py-2 rounded-lg transition-colors ${
                  location.pathname === '/despensa'
                    ? 'text-orange-600 bg-orange-50'
                    : 'text-gray-600 hover:text-orange-500'
                }`}
              >
                <Refrigerator size={24} />
                <span className="text-xs">Despensa</span>
              </button>

              <button
                onClick={() => navigate('/tiendas')}
                className={`flex flex-col items-center gap-1 px-4 py-2 rounded-lg transition-colors ${
                  location.pathname === '/tiendas'
                    ? 'text-orange-600 bg-orange-50'
                    : 'text-gray-600 hover:text-orange-500'
                }`}
              >
                <MapPin size={24} />
                <span className="text-xs">Tiendas</span>
              </button>

              <button
                onClick={() => navigate('/perfil')}
                className={`flex flex-col items-center gap-1 px-4 py-2 rounded-lg transition-colors ${
                  location.pathname === '/perfil'
                    ? 'text-orange-600 bg-orange-50'
                    : 'text-gray-600 hover:text-orange-500'
                }`}
              >
                <User size={24} />
                <span className="text-xs">Perfil</span>
              </button>
            </div>
          </nav>
        )}
      </div>
    </UserProvider>
  );
}
