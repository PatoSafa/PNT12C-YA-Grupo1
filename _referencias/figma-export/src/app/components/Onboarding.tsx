import { useState } from 'react';
import { useNavigate } from 'react-router';
import { ChefHat, ArrowRight, Check } from 'lucide-react';
import { useUser } from '../context/UserContext';

const restriccionesOptions = [
  'Vegetariano',
  'Vegano',
  'Sin Gluten',
  'Sin Lactosa',
  'Kosher',
  'Halal',
  'Bajo en Carbohidratos',
];

const alergiasOptions = [
  'Frutos Secos',
  'Mariscos',
  'Pescado',
  'Huevo',
  'Lácteos',
  'Soja',
  'Gluten',
  'Sésamo',
];

const gustosOptions = [
  'Italiana',
  'Mexicana',
  'Asiática',
  'Mediterránea',
  'Vegana',
  'Comfort Food',
  'Saludable',
  'Rápido',
];

export function Onboarding() {
  const navigate = useNavigate();
  const { setPreferences } = useUser();
  const [step, setStep] = useState(1);
  const [nombre, setNombre] = useState('');
  const [restricciones, setRestricciones] = useState<string[]>([]);
  const [alergias, setAlergias] = useState<string[]>([]);
  const [gustos, setGustos] = useState<string[]>([]);
  const [nivelCocina, setNivelCocina] = useState<'principiante' | 'intermedio' | 'avanzado'>('intermedio');
  const [otraRestriccion, setOtraRestriccion] = useState('');
  const [otraAlergia, setOtraAlergia] = useState('');

  const toggleSelection = (item: string, list: string[], setter: (list: string[]) => void) => {
    if (list.includes(item)) {
      setter(list.filter(i => i !== item));
    } else {
      setter([...list, item]);
    }
  };

  const agregarOtraRestriccion = () => {
    if (otraRestriccion.trim()) {
      setRestricciones([...restricciones, otraRestriccion.trim()]);
      setOtraRestriccion('');
    }
  };

  const agregarOtraAlergia = () => {
    if (otraAlergia.trim()) {
      setAlergias([...alergias, otraAlergia.trim()]);
      setOtraAlergia('');
    }
  };

  const eliminarRestriccion = (restriccion: string) => {
    setRestricciones(restricciones.filter(r => r !== restriccion));
  };

  const eliminarAlergia = (alergia: string) => {
    setAlergias(alergias.filter(a => a !== alergia));
  };

  const handleComplete = () => {
    setPreferences({
      nombre,
      restricciones,
      alergias,
      gustos,
      nivelCocina,
    });
    navigate('/home');
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center p-6">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-orange-500 rounded-full mb-4">
            <ChefHat size={40} className="text-white" />
          </div>
          <h1 className="text-3xl mb-2 text-orange-900">CenAPP</h1>
          <p className="text-gray-600">Tu asistente personal de cocina</p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl p-8 mb-6">
          <div className="flex justify-between mb-6">
            {[1, 2, 3, 4, 5].map((s) => (
              <div
                key={s}
                className={`w-10 h-10 rounded-full flex items-center justify-center ${
                  s <= step ? 'bg-orange-500 text-white' : 'bg-gray-200 text-gray-400'
                }`}
              >
                {s < step ? <Check size={20} /> : s}
              </div>
            ))}
          </div>

          {step === 1 && (
            <div>
              <h2 className="text-xl mb-4 text-gray-800">¿Cómo te llamas?</h2>
              <input
                type="text"
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                placeholder="Tu nombre"
                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-orange-500 focus:outline-none"
              />
            </div>
          )}

          {step === 2 && (
            <div>
              <h2 className="text-xl mb-4 text-gray-800">¿Tienes restricciones alimenticias?</h2>
              <div className="space-y-2 mb-4">
                {restriccionesOptions.map((restriccion) => (
                  <button
                    key={restriccion}
                    onClick={() => toggleSelection(restriccion, restricciones, setRestricciones)}
                    className={`w-full px-4 py-3 rounded-lg border-2 text-left transition-all ${
                      restricciones.includes(restriccion)
                        ? 'border-orange-500 bg-orange-50 text-orange-900'
                        : 'border-gray-200 hover:border-orange-300'
                    }`}
                  >
                    {restriccion}
                  </button>
                ))}
              </div>

              {restricciones.filter(r => !restriccionesOptions.includes(r)).length > 0 && (
                <div className="mb-4">
                  <p className="text-sm text-gray-600 mb-2">Restricciones personalizadas:</p>
                  <div className="flex flex-wrap gap-2">
                    {restricciones.filter(r => !restriccionesOptions.includes(r)).map((restriccion) => (
                      <span
                        key={restriccion}
                        className="px-3 py-1 bg-orange-100 text-orange-800 rounded-full text-sm flex items-center gap-2"
                      >
                        {restriccion}
                        <button
                          onClick={() => eliminarRestriccion(restriccion)}
                          className="hover:text-orange-900"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                  </div>
                </div>
              )}

              <div className="border-t-2 border-gray-200 pt-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Agregar otra restricción:
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={otraRestriccion}
                    onChange={(e) => setOtraRestriccion(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && agregarOtraRestriccion()}
                    placeholder="Ej: Sin azúcar, Bajo en sodio..."
                    className="flex-1 px-4 py-2 border-2 border-gray-200 rounded-lg focus:border-orange-500 focus:outline-none text-sm"
                  />
                  <button
                    onClick={agregarOtraRestriccion}
                    disabled={!otraRestriccion.trim()}
                    className="px-4 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed"
                  >
                    Agregar
                  </button>
                </div>
              </div>
            </div>
          )}

          {step === 3 && (
            <div>
              <h2 className="text-xl mb-4 text-gray-800">¿Tienes alguna alergia?</h2>
              <div className="space-y-2 mb-4">
                {alergiasOptions.map((alergia) => (
                  <button
                    key={alergia}
                    onClick={() => toggleSelection(alergia, alergias, setAlergias)}
                    className={`w-full px-4 py-3 rounded-lg border-2 text-left transition-all ${
                      alergias.includes(alergia)
                        ? 'border-red-500 bg-red-50 text-red-900'
                        : 'border-gray-200 hover:border-red-300'
                    }`}
                  >
                    {alergia}
                  </button>
                ))}
              </div>

              {alergias.filter(a => !alergiasOptions.includes(a)).length > 0 && (
                <div className="mb-4">
                  <p className="text-sm text-gray-600 mb-2">Alergias personalizadas:</p>
                  <div className="flex flex-wrap gap-2">
                    {alergias.filter(a => !alergiasOptions.includes(a)).map((alergia) => (
                      <span
                        key={alergia}
                        className="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm flex items-center gap-2"
                      >
                        {alergia}
                        <button
                          onClick={() => eliminarAlergia(alergia)}
                          className="hover:text-red-900"
                        >
                          ×
                        </button>
                      </span>
                    ))}
                  </div>
                </div>
              )}

              <div className="border-t-2 border-gray-200 pt-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Agregar otra alergia:
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={otraAlergia}
                    onChange={(e) => setOtraAlergia(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && agregarOtraAlergia()}
                    placeholder="Ej: Kiwi, Apio, Mostaza..."
                    className="flex-1 px-4 py-2 border-2 border-gray-200 rounded-lg focus:border-red-500 focus:outline-none text-sm"
                  />
                  <button
                    onClick={agregarOtraAlergia}
                    disabled={!otraAlergia.trim()}
                    className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed"
                  >
                    Agregar
                  </button>
                </div>
              </div>
            </div>
          )}

          {step === 4 && (
            <div>
              <h2 className="text-xl mb-4 text-gray-800">¿Qué tipo de cocina te gusta?</h2>
              <div className="space-y-2">
                {gustosOptions.map((gusto) => (
                  <button
                    key={gusto}
                    onClick={() => toggleSelection(gusto, gustos, setGustos)}
                    className={`w-full px-4 py-3 rounded-lg border-2 text-left transition-all ${
                      gustos.includes(gusto)
                        ? 'border-green-500 bg-green-50 text-green-900'
                        : 'border-gray-200 hover:border-green-300'
                    }`}
                  >
                    {gusto}
                  </button>
                ))}
              </div>
            </div>
          )}

          {step === 5 && (
            <div>
              <h2 className="text-xl mb-4 text-gray-800">¿Cuál es tu nivel de cocina?</h2>
              <div className="space-y-3">
                {[
                  { value: 'principiante', label: 'Principiante', desc: 'Recién empiezo' },
                  { value: 'intermedio', label: 'Intermedio', desc: 'Tengo experiencia básica' },
                  { value: 'avanzado', label: 'Avanzado', desc: 'Soy experimentado' },
                ].map((nivel) => (
                  <button
                    key={nivel.value}
                    onClick={() => setNivelCocina(nivel.value as typeof nivelCocina)}
                    className={`w-full px-4 py-4 rounded-lg border-2 text-left transition-all ${
                      nivelCocina === nivel.value
                        ? 'border-orange-500 bg-orange-50'
                        : 'border-gray-200 hover:border-orange-300'
                    }`}
                  >
                    <div className="font-medium text-gray-800">{nivel.label}</div>
                    <div className="text-sm text-gray-500">{nivel.desc}</div>
                  </button>
                ))}
              </div>
            </div>
          )}

          <div className="flex gap-3 mt-8">
            {step > 1 && (
              <button
                onClick={() => setStep(step - 1)}
                className="px-6 py-3 border-2 border-orange-500 text-orange-500 rounded-lg hover:bg-orange-50 transition-colors"
              >
                Atrás
              </button>
            )}

            <button
              onClick={() => step === 5 ? handleComplete() : setStep(step + 1)}
              disabled={step === 1 && !nombre}
              className="flex-1 flex items-center justify-center gap-2 px-6 py-3 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed"
            >
              {step === 5 ? 'Comenzar' : 'Siguiente'}
              <ArrowRight size={20} />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
