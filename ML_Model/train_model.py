import numpy as np
import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

print("1. Generando dataset sintético basado en distribuciones estadísticas...")
np.random.seed(42)

# Transacciones normales (90% de los datos)
# Simulamos montos bajos/medios y alta frecuencia
normal_amounts = np.random.normal(loc=500, scale=200, size=900)
normal_frequencies = np.random.normal(loc=5, scale=2, size=900)
normal_labels = np.zeros(900) # 0 = Seguro

# Transacciones fraudulentas (10% de los datos)
# Simulamos anomalías: montos muy altos (fuera de la desviación estándar) y baja frecuencia
fraud_amounts = np.random.normal(loc=15000, scale=3000, size=100)
fraud_frequencies = np.random.normal(loc=1, scale=0.5, size=100)
fraud_labels = np.ones(100) # 1 = Fraude

# Combinamos todo en un DataFrame
amounts = np.concatenate([normal_amounts, fraud_amounts])
frequencies = np.concatenate([normal_frequencies, fraud_frequencies])
labels = np.concatenate([normal_labels, fraud_labels])

df = pd.DataFrame({'Amount': amounts, 'Frequency24h': frequencies, 'IsFraud': labels})

# Limpiamos valores negativos imposibles en la vida real
df['Amount'] = df['Amount'].clip(lower=1)
df['Frequency24h'] = df['Frequency24h'].clip(lower=1)

X = df[['Amount', 'Frequency24h']].values.astype(np.float32) # ONNX requiere float32
y = df['IsFraud'].values.astype(np.int64)

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

print("2. Entrenando el modelo (Random Forest Classifier)...")
model = RandomForestClassifier(n_estimators=100, max_depth=5, random_state=42)
model.fit(X_train, y_train)

score = model.score(X_test, y_test)
print(f"Precisión del modelo en test: {score * 100:.2f}%")

print("3. Exportando el modelo matemático a formato ONNX...")
# Definimos el tensor de entrada: 1 fila, 2 columnas (Amount, Frequency24h)
initial_type = [('float_input', FloatTensorType([None, 2]))]

# Convertimos el modelo
onnx_model = convert_sklearn(model, initial_types=initial_type, target_opset=12)

# Guardamos el archivo binario
with open("fraud_model.onnx", "wb") as f:
    f.write(onnx_model.SerializeToString())

print("¡Éxito! Archivo 'fraud_model.onnx' generado y listo para ser consumido en .NET.")