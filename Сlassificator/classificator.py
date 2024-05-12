import keymaps
from utils import getTimestamp
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, LSTM, TimeDistributed, Flatten
from tensorflow.keras.utils import to_categorical

data = [
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D1"], 
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D1"], 
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D2"], 
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D2"], 
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D3"], 
    ["12.05.2024 18:15:26", "Keyboard", "Down", "D3",]  
    ["12.05.2024 18:15:26", "Keyboard", "Up", "D1",  ]  
    ["12.05.2024 18:15:26", "Keyboard", "Up", "D1"], 
    ["12.05.2024 18:15:26", "Keyboard", "Up", "D2"], 
    ["12.05.2024 18:15:26", "Keyboard", "Up", "D2"], 
    ["12.05.2024 18:15:27", "Keyboard", "Up", "D3"], 
    ["12.05.2024 18:15:27", "Keyboard", "Up", "D3"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "Left"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "Left"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "Right"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "Right"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "XButton2"], 
    ["12.05.2024 18:15:27", "Mouse", "Down", "XButton2"], 
]

def preprocess_data(data):
    # Пример данных: (timestamp, [(deviceNumber, buttonNumber, isPress), ...], targetValue)
    timestamps, sequences, targets = zip(*data)
    # Нормализация временных меток
    timestamps = np.array(timestamps) / np.max(timestamps)
    # Преобразование последовательностей
    sequences = [np.array([(dev_num, btn_num, int(is_press)) for dev_num, btn_num, is_press in seq]) for seq in sequences]
    # Выравнивание длины последовательностей
    max_len = max(len(seq) for seq in sequences)
    sequences_padded = np.array([np.pad(seq, ((0, max_len - len(seq)), (0, 0)), mode='constant') for seq in sequences])
    # Преобразование целевых меток
    targets = np.array(targets)
    return timestamps, sequences_padded, targets


def build_model(input_shape):
    model = Sequential([
        LSTM(50, input_shape=input_shape, return_sequences=True),
        TimeDistributed(Dense(10, activation='relu')),
        Flatten(),
        Dense(1, activation='sigmoid')
    ])
    model.compile(optimizer='adam', loss='mean_squared_error', metrics=['accuracy'])
    return model

# Пример данных для демонстрации
example_data = [
    (1234567890, [(1, 2, True), (2, 3, False)], 0.8),
    (1234567891, [(3, 1, True)], 0.3)
]

timestamps, sequences_padded, targets = preprocess_data(example_data)
model = build_model(sequences_padded[0].shape)
model.fit([timestamps, sequences_padded], targets, epochs=10, batch_size=2)

test_data = [
    (1234567895, [(1, 3, True), (3, 3, True)], 0.9)
]
test_timestamps, test_sequences_padded, test_targets = preprocess_data(test_data)
loss, accuracy = model.evaluate([test_timestamps, test_sequences_padded], test_targets)
print(f"Точность модели: {accuracy}")