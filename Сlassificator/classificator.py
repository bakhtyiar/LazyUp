import keymaps
from utils import getTimestamp

import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense

data = [
    ["01.05.2024 12:29:00", "Key", "D3"],
    ["01.05.2024 12:29:00", "Key", "D4"],
    ["01.05.2024 12:29:00", "Mouse", "left_btn"],
    ["01.05.2024 12:29:00", "Mouse", "right_btn"]
]

# Преобразование данных для обучения в числовой формат (one-hot encoding)
# Пример представления данных: [0, 1] - "Key", [1, 0] - "Mouse", [0, 0, 1] - "D3", и т.д.
feature_dict = {"Key": [0, 1], "Mouse": [1, 0]}
deviceKey_dict = {
    "D3": [0, 0, 1],
    "D4": [0, 1, 0],
    "left_btn": [1, 0, 0],
    "right_btn": [0, 1, 0]
}

# Преобразование даты и времени в числовой формат. Можно также извлечь признаки из даты и времени.
# Это зависит от особенностей задачи.
# Пример кода:
# datetime_feature = [parse_datetime(item[0]) for item in data]
# Далее создадим модель нейронной сети:
# python

model = Sequential()
model.add(Dense(64, input_dim=5, activation='relu'))
model.add(Dense(32, activation='relu'))
model.add(Dense(2, activation='softmax'))  # Для целевой метки "а" или "б"
# Компиляция и обучение модели:
# python

model.compile(loss='categorical_crossentropy', optimizer='adam', metrics=['accuracy'])

# Подготовка данных для обучения
X_train = []  # Здесь должны быть данные с признаками
Y_train = []  # Здесь должны быть соответствующие метки

model.fit(X_train, Y_train, epochs=10, batch_size=32)  # Пример параметров обучения
# Важно учитывать плотность расположения по datetime записей при формировании датасета и подготовке признаков для обучения 1 .
# Это основной каркас для создания классификатора на Keras, используя предоставленные данные и учитывая плотность расположения записей по datetime.