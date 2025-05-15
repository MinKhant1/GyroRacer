// Include necessary libraries
#include <MPU6050.h>          // Library for interacting with MPU6050 accelerometer and gyroscope
#include <Wire.h>             // I2C communication library
#include <SoftwareWire.h>     // Allows software (bit-banged) I2C on arbitrary pins
#include <U8g2lib.h>          // Library for controlling OLED displays using U8G2

// Create MPU6050 sensor object
MPU6050 mpu;

// Create a software I2C interface on pins A2 (SDA) and A3 (SCL)
SoftwareWire myWire(A2, A3);

// Create a U8G2 object for a 128x64 SSD1306 OLED using the software I2C
U8G2_SSD1306_128X64_NONAME_1_SW_I2C u8g2(
  U8G2_R0, /* clock=*/ A3, /* data=*/ A2, /* reset=*/ U8X8_PIN_NONE
);

// Define pin numbers for inputs and outputs
const int forwardPin = 7;
const int backwardPin = 6;
const int brakePin = 5;
const int greenLedPin = 4;
const int redLedPin = 3;
const int buzzerPin = 2;

// Variable to hold odor level received via serial command
int odorLevel = 0;

void setup() {
  Serial.begin(9600);         // Start serial communication

  Wire.begin();               // Initialize hardware I2C (used by MPU6050)
  myWire.begin();             // Initialize software I2C (used by OLED)
  u8g2.begin();               // Initialize the OLED display

  // Set control pins as input with pull-up resistors
  pinMode(forwardPin, INPUT_PULLUP);
  pinMode(backwardPin, INPUT_PULLUP);
  pinMode(brakePin, INPUT_PULLUP);

  // Set output pins
  pinMode(buzzerPin, OUTPUT);
  pinMode(greenLedPin, OUTPUT);
  pinMode(redLedPin, OUTPUT);

  digitalWrite(buzzerPin, HIGH); // Set buzzer to idle (HIGH = off with LOW-level trigger)

  Wire.begin();            // (Repeated, optional) Initialize I2C bus again
  mpu.initialize();        // Initialize MPU6050 sensor

  // Clear and draw initial "0" on OLED
  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_logisoso32_tr);
  u8g2.drawStr(0, 24, "0");
  u8g2.sendBuffer();
}

void loop() {
  // Read the states of the control buttons
  int forwardState = digitalRead(forwardPin);
  int backwardState = digitalRead(backwardPin);
  int brakeState = digitalRead(brakePin);

  // If forward button is pressed
  if (forwardState == LOW) {
    Serial.println("Forward");
    digitalWrite(greenLedPin, HIGH);  // Turn on green LED
  } else {
    digitalWrite(greenLedPin, LOW);   // Turn off green LED
  }

  // If backward button is pressed
  if (backwardState == LOW) {
    Serial.println("Backward");
  }

  // If brake button is pressed
  if (brakeState == LOW) {
    Serial.println("Brake");
    digitalWrite(redLedPin, HIGH);    // Turn on red LED
  } else {
    digitalWrite(redLedPin, LOW);     // Turn off red LED
  }

  // Handle incoming serial commands
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');  // Read until newline
    command.trim();                                 // Remove extra whitespace
    Serial.print("Received command: ");
    Serial.println(command);

    if (command == "BUZZ") {
      
      digitalWrite(buzzerPin, LOW);
      delay(200);                 // Buzzer on for 200 milliseconds
      digitalWrite(buzzerPin, HIGH);
    }

    odorUpdate(command);         // Pass command to check if it's an odor update
  }

  // Read acceleration and gyroscope data from MPU6050
  int16_t ax, ay, az, gx, gy, gz;
  mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

  // Send motion data to serial in CSV format
  Serial.print(ax); Serial.print(",");
  Serial.print(ay); Serial.print(",");
  Serial.print(az); Serial.print(",");
  Serial.print(gx); Serial.print(",");
  Serial.print(gy); Serial.print(",");
  Serial.println(gz);

  delay(50); // Small delay to limit data rate
}

// Helper function to update the OLED when an ODOR command is received
int lastDisplayedOdor = -1; // Not used yet, could be used for optimization

void odorUpdate(String command) {
  if (command.startsWith("ODOR:")) {
    // Extract the numeric part of the command
    int newOdorLevel = command.substring(5).toInt();
    odorLevel = newOdorLevel;

    // Convert to string for display
    String odorText = String(odorLevel);
    const char* text = odorText.c_str();

    // Display the odor value centered on the OLED screen
    u8g2.firstPage();
    do {
      u8g2.setFont(u8g2_font_logisoso32_tr);
      int textWidth = u8g2.getStrWidth(text);
      int x = (128 - textWidth) / 2;
      int y = (64 + 32) / 2 - 2; // Center vertically
      u8g2.setCursor(x, y);
      u8g2.print(text);
    } while (u8g2.nextPage());
  }
}
