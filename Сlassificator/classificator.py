import keymaps
import utils
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, LSTM, TimeDistributed, Flatten
from tensorflow.keras.utils import to_categorical
import os

# Путь к директории с файлами
defaultLogsDirectory = "./logs"

def parseData(directory):
    ret = []
    for filename in os.listdir(directory):
        if filename.endswith(".txt"):
            with open(os.path.join(directory, filename), 'r') as file:
                modeName = filename.split('_')[0]
                for line in file:
                    data = utils.splitRec(line)
                    data.append(modeName)
                    ret.append = data
    return ret

def remapData(data):
    for line in data:
        line[0] = utils.getTimestamp(line[0])
        line[1] = keymaps.DeviceKey.get(line[1], line[1])
        line[2] = keymaps.ButtonDirection.get(line[2], line[2])
        if (line[1] == keymaps.DeviceKey["Keyboard"]):
            line[3] = keymaps.KeyboardButtons.get(line[3], line[3])
        else:
            line[3] = keymaps.MouseButtons.get(line[3], line[3])
        line[4] = keymaps.ModeNames.get(line[4], line[4])
    return data

# remapData() Пример полученных данных
# data = [
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D1", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D1", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D2", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D2", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D3", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Down", "D3", "Disturb"]  
#     ["12.05.2024 18:15:26", "Keyboard", "Up", "D1", "Disturb"]  
#     ["12.05.2024 18:15:26", "Keyboard", "Up", "D1", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Up", "D2", "Disturb"], 
#     ["12.05.2024 18:15:26", "Keyboard", "Up", "D2", "Disturb"], 
#     ["12.05.2024 18:15:27", "Keyboard", "Up", "D3", "Disturb"], 
#     ["12.05.2024 18:15:27", "Keyboard", "Up", "D3", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "Left", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "Left", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "Right", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "Right", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "XButton2", "Disturb"], 
#     ["12.05.2024 18:15:27", "Mouse", "Down", "XButton2", "Disturb"], 
# ]

def preprocess_data(directory):
    parsedData = parseData(directory)
    print("parsedData")
    print(parsedData)
    mappedData = remapData(parsedData)
    print("mappedData")
    print(mappedData)
    # Пример данных: (timestamp, [(deviceNumber, buttonNumber, isPress), ...], targetValue)
    timestamps, sequences, targets = zip(*mappedData)
    print("timestamps")
    print(timestamps)
    print("sequences")
    print(sequences)
    print("targets")
    print(targets)
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

timestamps, sequences_padded, targets = preprocess_data(defaultLogsDirectory)
model = build_model(sequences_padded[0].shape)
model.fit([timestamps, sequences_padded], targets, epochs=10, batch_size=2)
# test_timestamps, test_sequences_padded, test_targets = preprocess_data(test_data)
# loss, accuracy = model.evaluate([test_timestamps, test_sequences_padded], test_targets)
# print(f"Точность модели: {accuracy}")