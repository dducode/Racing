<h1 align="center">Racing</h1>
<h1 align="center">

![RacingImg](Assets/Textures/Icons/icon.png)

[Racing (Demo)](https://play.unity.com/mg/other/racing-demo-5)

</h1>

# Содержание

* [Описание](#описание)
    * [Автомобили](#автомобили)
      * [Sport](#sport)
      * [Civilian](#civilian)
      * [Mustang](#mustang)
    * [Как играть](#как-играть)     
    * [Игровой режим](#игровой-режим)
      * [Трасса](#трасса)
      * [Мини карта](#мини-карта)
      * [Пауза](#пауза)
* [Использованные ассеты](#использованные-ассеты)

# Описание

Демонстрационный проект на Unity (v.2021.3.6f1), 
в котором вы можете выбрать один из трёх автомобилей и 
перемещаться на нём по сгенерированной трассе.

![](For_github/Screenshot_7.png)

## Автомобили

Всего в игре 3 вида автомобилей. В окне select вы можете выбрать любой из
них, какой вам понравится, и покататься на нём. Характеристики
каждого автомобиля указаны на панели в левом верхнем углу.
Для открытия панели нужно нажать на кнопку "инфо"

### Sport

![](For_github/Screenshot_12.png)

### Civilian

![](For_github/Screenshot_1.png)

### Mustang

![](For_github/Screenshot_3.png)

## Как играть

Управляйте автомобилем с помощью клавиш WASD. В окне 
настроек игры вы можете установить разрешение игры и 
качество графики, а также выбрать клавиши для 
включения передних фар и для торможения.

![](For_github/Screenshot_5.png)

Нажатие на клавишу F выводит консоль в левом верхнем углу 
с отображением текущей частоты кадров (только в игровом режиме).

![](For_github/Screenshot_6.png)

## Игровой режим

### Трасса

Трасса генерируется процедурно из заранее готовых чанков.
Во время генерации всплывает окно с прогресс-баром, указывающим
на готовность сгенерированной дороги. Как только трасса будет
готова, на ней можно будет ездить

![](For_github/Screenshot_4.png)

### Мини карта

В правом нижнем углу есть мини-карта, она служит в качестве
ориентира на трассе. Чем быстрее вы едите, тем больше
радиус обзора мини-карты

![](For_github/Screenshot_13.png)

Как только вы доедите до финиша, вас отбросит на старт и
генерация трассы начнётся заново

### Пауза

В игровом режиме вы можете поставить игру на паузу. В окне
паузы вам будет предложено 3 действия - продолжить игру,
выйти в главное меню, либо выйти из игры. Также вы можете
открыть настройки, нажав на кнопку в правом верхнем углу

![](For_github/Screenshot_14.png)

# Использованные ассеты

* Автомобили - [раз](https://assetstore.unity.com/packages/3d/vehicles/land/low-poly-sports-car-20-144253), [два](https://assetstore.unity.com/packages/3d/vehicles/land/low-poly-civilian-vehicle-5-124987), [три](https://assetstore.unity.com/packages/3d/vehicles/land/realistic-mobile-car-demo-173467).
* [Дорожные элементы](https://assetstore.unity.com/packages/3d/props/simple-street-props-194706), [заборы](https://assetstore.unity.com/packages/3d/chainlink-fences-73107)
* [Модульная система дорог](https://assetstore.unity.com/packages/3d/environments/urban/modular-lowpoly-streets-free-192094)
* [UI](https://assetstore.unity.com/packages/2d/gui/icons/simple-button-set-02-184903)