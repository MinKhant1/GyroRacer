# 🚗 Arduino Car Control System with MPU6050 & OLED Display

This project integrates motion sensing, display output, and basic control for a vehicle-like system using an Arduino. It supports communication with a Unity game or application via serial using two C# scripts: **CarController.cs** and **ArduinoInput.cs**.

---

## 📁 Project Structure

### Arduino Files

- **`car_controller.ino`** – Main Arduino sketch: handles sensor input, serial commands, and hardware output (LEDs, buzzer, OLED).
- **MPU6050** – Motion sensor used to track acceleration and rotation.
- **OLED Display (U8g2)** – Displays odor level via a 128x64 OLED screen connected with Software I2C.

### Unity Scripts

- **`CarController.cs`** – Handles vehicle movement based on sensor data from the Arduino.
- **`ArduinoInput.cs`** – Reads serial data from Arduino and sends commands (e.g., "BUZZ", "ODOR:50") back.

---

## 🛠 Hardware Requirements

- Arduino Uno (or compatible)
- MPU6050 Sensor
- SSD1306 128x64 OLED Display
- 3 Buttons (Forward, Backward, Brake)
- 2 LEDs (Green for forward, Red for brake)
- 1 Buzzer (Low-level triggered)
- Jumper wires, Breadboard

---

## 🔌 Pin Configuration

| Component       | Arduino Pin       |
| --------------- | ----------------- |
| Forward Button  | D7                |
| Backward Button | D6                |
| Brake Button    | D5                |
| Green LED       | D4                |
| Red LED         | D3                |
| Buzzer          | D2                |
| OLED SDA        | A2                |
| OLED SCL        | A3                |
| **MPU6050 SDA** | A4 (default Wire) |
| **MPU6050 SCL** | A5 (default Wire) |

---

## 📦 Required Libraries

Install via Arduino Library Manager:

- [`MPU6050`](https://github.com/ElectronicCats/mpu6050) by Electronic Cats
- [`SoftwareWire`](https://github.com/Testato/SoftwareWire) by Testato
- [`U8g2`](https://github.com/olikraus/u8g2) by Oliver
- `Wire` (built-in)

---

## 🧠 Features

- MPU6050 gyroscope/accelerometer readings sent via serial.
- Display odor levels on OLED using serial commands.
- Control input via push buttons with LED feedback.
- Trigger buzzer via serial command (`BUZZ`).
- Fully compatible with Unity for interactive applications.

---

## 🔄 Serial Commands

| Command        | Description                                  |
| -------------- | -------------------------------------------- |
| `BUZZ`         | Triggers buzzer for 200 ms                   |
| `ODOR:<value>` | Displays `<value>` on OLED (e.g., `ODOR:45`) |

---

## 📦 Unity Integration

Make sure your Unity project contains:

- `CarController.cs`: Handles physics/movement based on sensor input (e.g., steering from `gz` gyro data).
- `ArduinoInput.cs`: Manages serial communication, sending commands like `"BUZZ"` or `"ODOR:27"` and reading sensor streams like `ax,ay,az,gx,gy,gz`.

---

## 📌 Future Improvements

- Add steering control via `gz` gyro axis.
- Optimize OLED refresh to avoid flickering.
- Add graphical bar or warning threshold for odor levels.
- Integrate PWM motor speed control.

---

## 🧪 Author & License

Created by [Sai Wai Yan Phyo & Min Khant].  
MIT License.
