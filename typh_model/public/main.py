import sys
import cv2
import numpy as np
from PIL import Image
from keras.models import load_model
from flask import Flask, request, jsonify
import requests
from io import BytesIO
import base64

app = Flask(__name__)

model = load_model('resnet_large_model.h5')
model.load_weights('resnet_large_weights.h5')

def download_image(url):
    response = requests.get(url)
    if response.status_code == 200:
        img = Image.open(BytesIO(response.content))
        if img.mode == 'RGBA':
            img = img.convert('RGB')
        return np.array(img)
    else:
        return None

def preprocess_image(image):
    image_resized = cv2.resize(image, (224, 224))
    image_resized = image_resized.astype(np.float32) / 255.0
    image_resized = np.expand_dims(image_resized, axis=0)
    return image_resized

def generate_triggering_mask(image, threshold=0.4):
    predictions = model.predict(image)
    triggering_mask = np.zeros((224, 224), dtype=np.uint8)

    pred_probabilities = predictions[0]
    predicted_class = np.argmax(pred_probabilities)

    if predicted_class == 1 and pred_probabilities[predicted_class] > threshold:
        triggering_mask[40:180, 40:180] = 255 

    kernel = np.ones((15, 15), np.uint8)
    triggering_mask = cv2.dilate(triggering_mask, kernel, iterations=2)

    return triggering_mask, predicted_class

def detect_bounding_box_from_mask(image, mask):
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    if contours:
        x, y, w, h = cv2.boundingRect(contours[0])
        color = (0, 255, 0)
        thickness = 1
        cv2.rectangle(image, (x, y), (x + w, y + h), color, thickness)
        highlighted_area = w * h
        return image, highlighted_area, (x, y, w, h)

    return image, 0, None

def predict_and_highlight_areas(image):
    original_image = image.copy()
    image_resized = preprocess_image(image)

    triggering_mask, predicted_class = generate_triggering_mask(image_resized)

    highlighted_image = original_image.copy()
    highlighted_area = 0

    if predicted_class == 1:
        highlighted_image, highlighted_area, bounding_box = detect_bounding_box_from_mask(highlighted_image, triggering_mask)

    _, highlighted_buffer = cv2.imencode('.jpeg', highlighted_image)
    highlighted_image_bytes = base64.b64encode(highlighted_buffer).decode('utf-8')

    _, original_buffer = cv2.imencode('.jpeg', original_image)
    original_image_bytes = base64.b64encode(original_buffer).decode('utf-8')

    full_image_area = original_image.shape[1] * original_image.shape[0] 

    highlighted_percentage = (highlighted_area / full_image_area * 100) if full_image_area > 0 else 0

    return predicted_class, highlighted_area, full_image_area, highlighted_percentage, highlighted_image_bytes, original_image_bytes

@app.route('/predict', methods=['POST'])
def predict():
    data = request.get_json()  

    if 'image_url' not in data:
        return jsonify({'error': 'No image URL provided'}), 400

    image_url = data['image_url']

    image = download_image(image_url)

    if image is None:
        return jsonify({'error': 'Failed to retrieve or decode image from URL'}), 400


    predicted_class, highlighted_area, full_image_area, highlighted_percentage, highlighted_image_bytes, original_image_bytes = predict_and_highlight_areas(image)


    predictions = model.predict(preprocess_image(image))
    predicted_probability = predictions[0][predicted_class]


    return jsonify({
        'Is_Trypophobia_TriggerImage': int(predicted_class),
        'TriggerPercentage': float(predicted_probability) * 100,
        'highlighted_area': highlighted_area,
        'full_image_area': full_image_area,
        'highlighted_percentage': highlighted_percentage,
        'highlighted_image': highlighted_image_bytes,
        'original_image': original_image_bytes
    }), 200

if __name__ == '__main__':
    app.run(port=8080)
