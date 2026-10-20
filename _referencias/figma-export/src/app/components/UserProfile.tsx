import { useState } from 'react';
import { User, Edit2, Check, X, ChefHat, Heart, AlertCircle, Utensils } from 'lucide-react';
import { useUser } from '../context/UserContext';

export function UserProfile() {
  const { preferences, setPreferences } = useUser();
  const [isEditing, setIsEditing] = useState(false);
  const [editedPrefs, setEditedPrefs] = useState(preferences);
  const [nuevaRestriccion, setNuevaRestriccion] = useState('');
  const [nuevaAlergia, setNuevaAlergia] = useState('');

  const handleSave = () => {
    if (editedPrefs) {
      setPreferences(editedPrefs);
      setIsEditing(false);
      setNuevaRestriccion('');
      setNuevaAlergia('');
    }
  };

  const handleCancel = () => {
    setEditedPrefs(preferences);
    setIsEditing(false);
    setNuevaRestriccion('');
    setNuevaAlergia('');
  };

  const toggleItem = (category: 'restricciones' | 'alergias' | 'gustos', item: string) => {
    if (!editedPrefs) return;
    const current = editedPrefs[category];
    const updated = current.includes(item)
      ? current.filter(i => i !== item)
      : [...current, item];
    setEditedPrefs({ ...editedPrefs, [category]: updated });
  };

  const agregarRestriccion = () => {
    if (!editedPrefs || !nuevaRestriccion.trim()) return;
    setEditedPrefs({
      ...editedPrefs,
      restricciones: [...editedPrefs.restricciones, nuevaRestriccion.trim()]
    });
    setNuevaRestriccion('');
  };

  const agregarAlergia = () => {
    if (!editedPrefs || !nuevaAlergia.trim()) return;
    setEditedPrefs({
      ...editedPrefs,
      alergias: [...editedPrefs.alergias, nuevaAlergia.trim()]
    });
    setNuevaAlergia('');
  };

  const eliminarItem = (category: 'restricciones' | 'alergias', item: string) => {
    if (!editedPrefs) return;
    setEditedPrefs({
      ...editedPrefs,
      [category]: editedPrefs[category].filter(i => i !== item)
    });
  };

  if (!preferences) {
    return (
      <div className="min-h-screen flex items-center justify-center pb-24">
        <p className="text-gray-500">Cargando perfil...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen pb-24">
      <div className="bg-gradient-to-r from-purple-500 to-pink-500 text-white p-6 pb-8">
        <div className="max-w-lg mx-auto">
          <div className="flex items-center justify-between mb-6">
            <h1 className="text-2xl">Mi Perfil</h1>
            {!isEditing ? (
              <button
                onClick={() => setIsEditing(true)}
                className="flex items-center gap-2 px-4 py-2 bg-white/20 rounded-lg hover:bg-white/30 transition-colors"
              >
                <Edit2 size={18} />
                <span>Editar</span>
              </button>
            ) : (
              <div className="flex gap-2">
                <button
                  onClick={handleSave}
                  className="flex items-center gap-1 px-3 py-2 bg-green-500 rounded-lg hover:bg-green-600 transition-colors"
                >
                  <Check size={18} />
                </button>
                <button
                  onClick={handleCancel}
                  className="flex items-center gap-1 px-3 py-2 bg-red-500 rounded-lg hover:bg-red-600 transition-colors"
                >
                  <X size={18} />
                </button>
              </div>
            )}
          </div>

          <div className="flex items-center gap-4">
            <div className="w-20 h-20 bg-white/20 rounded-full flex items-center justify-center">
              <User size={40} />
            </div>
            <div>
              {isEditing ? (
                <input
                  type="text"
                  value={editedPrefs?.nombre || ''}
                  onChange={(e) => setEditedPrefs(prev => prev ? { ...prev, nombre: e.target.value } : prev)}
                  className="text-2xl bg-white/20 border-2 border-white/40 rounded-lg px-3 py-1 focus:outline-none focus:border-white"
                />
              ) : (
                <h2 className="text-2xl">{preferences.nombre}</h2>
              )}
              <p className="text-purple-100">
                Nivel: {preferences.nivelCocina.charAt(0).toUpperCase() + preferences.nivelCocina.slice(1)}
              </p>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-lg mx-auto px-6 -mt-4">
        <div className="bg-white rounded-xl shadow-lg p-6 mb-6">
          <div className="flex items-center gap-2 mb-4">
            <ChefHat size={20} className="text-purple-600" />
            <h3 className="font-medium text-gray-800">Nivel de Cocina</h3>
          </div>
          {isEditing ? (
            <div className="space-y-2">
              {[
                { value: 'principiante', label: 'Principiante' },
                { value: 'intermedio', label: 'Intermedio' },
                { value: 'avanzado', label: 'Avanzado' },
              ].map(nivel => (
                <button
                  key={nivel.value}
                  onClick={() => setEditedPrefs(prev => prev ? { ...prev, nivelCocina: nivel.value as any } : prev)}
                  className={`w-full px-4 py-2 rounded-lg border-2 text-left transition-all ${
                    editedPrefs?.nivelCocina === nivel.value
                      ? 'border-purple-500 bg-purple-50 text-purple-900'
                      : 'border-gray-200 hover:border-purple-300'
                  }`}
                >
                  {nivel.label}
                </button>
              ))}
            </div>
          ) : (
            <p className="text-gray-600 capitalize">{preferences.nivelCocina}</p>
          )}
        </div>

        <div className="bg-white rounded-xl shadow-lg p-6 mb-6">
          <div className="flex items-center gap-2 mb-4">
            <Utensils size={20} className="text-orange-600" />
            <h3 className="font-medium text-gray-800">Restricciones Alimenticias</h3>
          </div>
          {isEditing ? (
            <div className="space-y-4">
              <div className="flex flex-wrap gap-2">
                {['Vegetariano', 'Vegano', 'Sin Gluten', 'Sin Lactosa', 'Kosher', 'Halal'].map(restriccion => (
                  <button
                    key={restriccion}
                    onClick={() => toggleItem('restricciones', restriccion)}
                    className={`px-3 py-1 rounded-full text-sm transition-all ${
                      editedPrefs?.restricciones.includes(restriccion)
                        ? 'bg-orange-500 text-white'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    {restriccion}
                  </button>
                ))}
              </div>

              {editedPrefs?.restricciones.filter(r => !['Vegetariano', 'Vegano', 'Sin Gluten', 'Sin Lactosa', 'Kosher', 'Halal'].includes(r)).length > 0 && (
                <div>
                  <p className="text-xs text-gray-500 mb-2">Restricciones personalizadas:</p>
                  <div className="flex flex-wrap gap-2">
                    {editedPrefs.restricciones.filter(r => !['Vegetariano', 'Vegano', 'Sin Gluten', 'Sin Lactosa', 'Kosher', 'Halal'].includes(r)).map(restriccion => (
                      <span
                        key={restriccion}
                        className="px-3 py-1 bg-orange-100 text-orange-800 rounded-full text-sm flex items-center gap-2"
                      >
                        {restriccion}
                        <button
                          onClick={() => eliminarItem('restricciones', restriccion)}
                          className="hover:text-orange-900 font-bold"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                  </div>
                </div>
              )}

              <div className="border-t pt-3">
                <label className="block text-xs text-gray-600 mb-2">Agregar otra restricción:</label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={nuevaRestriccion}
                    onChange={(e) => setNuevaRestriccion(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && agregarRestriccion()}
                    placeholder="Ej: Sin azúcar, Bajo en sodio..."
                    className="flex-1 px-3 py-2 border-2 border-gray-200 rounded-lg focus:border-orange-500 focus:outline-none text-sm"
                  />
                  <button
                    onClick={agregarRestriccion}
                    disabled={!nuevaRestriccion.trim()}
                    className="px-3 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed text-sm"
                  >
                    Agregar
                  </button>
                </div>
              </div>
            </div>
          ) : preferences.restricciones.length > 0 ? (
            <div className="flex flex-wrap gap-2">
              {preferences.restricciones.map(restriccion => (
                <span key={restriccion} className="px-3 py-1 bg-orange-100 text-orange-800 rounded-full text-sm">
                  {restriccion}
                </span>
              ))}
            </div>
          ) : (
            <p className="text-gray-400">Ninguna</p>
          )}
        </div>

        <div className="bg-white rounded-xl shadow-lg p-6 mb-6">
          <div className="flex items-center gap-2 mb-4">
            <AlertCircle size={20} className="text-red-600" />
            <h3 className="font-medium text-gray-800">Alergias</h3>
          </div>
          {isEditing ? (
            <div className="space-y-4">
              <div className="flex flex-wrap gap-2">
                {['Frutos Secos', 'Mariscos', 'Pescado', 'Huevo', 'Lácteos', 'Soja', 'Gluten'].map(alergia => (
                  <button
                    key={alergia}
                    onClick={() => toggleItem('alergias', alergia)}
                    className={`px-3 py-1 rounded-full text-sm transition-all ${
                      editedPrefs?.alergias.includes(alergia)
                        ? 'bg-red-500 text-white'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    {alergia}
                  </button>
                ))}
              </div>

              {editedPrefs?.alergias.filter(a => !['Frutos Secos', 'Mariscos', 'Pescado', 'Huevo', 'Lácteos', 'Soja', 'Gluten'].includes(a)).length > 0 && (
                <div>
                  <p className="text-xs text-gray-500 mb-2">Alergias personalizadas:</p>
                  <div className="flex flex-wrap gap-2">
                    {editedPrefs.alergias.filter(a => !['Frutos Secos', 'Mariscos', 'Pescado', 'Huevo', 'Lácteos', 'Soja', 'Gluten'].includes(a)).map(alergia => (
                      <span
                        key={alergia}
                        className="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm flex items-center gap-2"
                      >
                        {alergia}
                        <button
                          onClick={() => eliminarItem('alergias', alergia)}
                          className="hover:text-red-900 font-bold"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                  </div>
                </div>
              )}

              <div className="border-t pt-3">
                <label className="block text-xs text-gray-600 mb-2">Agregar otra alergia:</label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={nuevaAlergia}
                    onChange={(e) => setNuevaAlergia(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && agregarAlergia()}
                    placeholder="Ej: Kiwi, Apio, Mostaza..."
                    className="flex-1 px-3 py-2 border-2 border-gray-200 rounded-lg focus:border-red-500 focus:outline-none text-sm"
                  />
                  <button
                    onClick={agregarAlergia}
                    disabled={!nuevaAlergia.trim()}
                    className="px-3 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed text-sm"
                  >
                    Agregar
                  </button>
                </div>
              </div>
            </div>
          ) : preferences.alergias.length > 0 ? (
            <div className="flex flex-wrap gap-2">
              {preferences.alergias.map(alergia => (
                <span key={alergia} className="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm">
                  {alergia}
                </span>
              ))}
            </div>
          ) : (
            <p className="text-gray-400">Ninguna</p>
          )}
        </div>

        <div className="bg-white rounded-xl shadow-lg p-6 mb-6">
          <div className="flex items-center gap-2 mb-4">
            <Heart size={20} className="text-pink-600" />
            <h3 className="font-medium text-gray-800">Gustos Culinarios</h3>
          </div>
          {isEditing ? (
            <div className="flex flex-wrap gap-2">
              {['Italiana', 'Mexicana', 'Asiática', 'Mediterránea', 'Vegana', 'Comfort Food', 'Saludable', 'Rápido'].map(gusto => (
                <button
                  key={gusto}
                  onClick={() => toggleItem('gustos', gusto)}
                  className={`px-3 py-1 rounded-full text-sm transition-all ${
                    editedPrefs?.gustos.includes(gusto)
                      ? 'bg-pink-500 text-white'
                      : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                  }`}
                >
                  {gusto}
                </button>
              ))}
            </div>
          ) : preferences.gustos.length > 0 ? (
            <div className="flex flex-wrap gap-2">
              {preferences.gustos.map(gusto => (
                <span key={gusto} className="px-3 py-1 bg-pink-100 text-pink-800 rounded-full text-sm">
                  {gusto}
                </span>
              ))}
            </div>
          ) : (
            <p className="text-gray-400">No especificado</p>
          )}
        </div>

        <div className="bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl p-6 border-2 border-purple-200">
          <h4 className="font-medium text-gray-800 mb-2">💡 Consejo</h4>
          <p className="text-sm text-gray-600">
            Mantén tu perfil actualizado para recibir las mejores recomendaciones de recetas
            personalizadas según tus preferencias y necesidades.
          </p>
        </div>
      </div>
    </div>
  );
}
