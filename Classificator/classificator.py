import keymaps
import utils
import numpy as np
import tensorflow as tf
import numpy as np
from keras.models import Sequential
from keras.layers import Dense
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
                    ret.append(data)
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
    # Преобразование в np.array
    data_array = np.array(mappedData)
    # Разделение на X и Y
    X = data_array[:, 1:4]  # Выбор столбцов 1-4 (индексы 1, 2, 3, 4)
    Y = data_array[:, 4]  # Выбор столбца 4 (индекс 4)
    return X, Y

def build_model(input_shape):
    model = Sequential([
        LSTM(50, input_shape=input_shape, return_sequences=True),
        TimeDistributed(Dense(10, activation='relu')),
        Flatten(),
        Dense(1, activation='sigmoid')
    ])
    model.compile(optimizer='adam', loss='mean_squared_error', metrics=['accuracy'])
    return model

# preprocess_data(defaultLogsDirectory)
X, y = preprocess_data(defaultLogsDirectory)
print("X")
print(X)
print("Y")
print(y)
# Построение модели
model = Sequential()
model.add(Dense(64, input_shape=(X.shape[1],), activation='relu'))  # Пример слоя Dense
model.add(Dense(1, activation='sigmoid'))  # Выходной слой
# Компиляция модели
model.compile(optimizer='adam', loss='binary_crossentropy', metrics=['accuracy'])  # Функция потерь и метрика
# Обучение модели
model.fit(X, y, epochs=10, batch_size=32, validation_split=0.2)  # Пример обучения на данных
# Получение вероятностей
predictions = model.predict(X)  # Получение предсказаний модели для ваших данных