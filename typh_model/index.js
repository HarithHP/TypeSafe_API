const express = require('express');
const fs = require('fs');
const path = require('path');
const axios = require('axios');

const app = express();

app.use(express.json({ limit: '10mb' }));

app.post('/upload', async (req, res) => {
    const { image } = req.body;

    if (!image) {
        return res.status(400).send('Image not provided');
    }

    const fileExtension = 'png';
    const filename = `uploaded_image_${Date.now()}.${fileExtension}`;
    const filePath = path.join(__dirname, 'uploads', filename);

    fs.writeFile(filePath, image, { encoding: 'base64' }, (err) => {
        if (err) {
            console.error('Error saving the file:', err);
            return res.status(500).send('Error occurred while saving the file');
        }

        const imageUrl = `http://localhost:3000/uploads/${filename}`;

        axios.post('http://127.0.0.1:8080/predict', {
            image_url: imageUrl
        })
        .then(response => {
            res.json(response.data);
        })
        .catch(error => {
            console.error('Error in Axios request:', error.response ? error.response.data : error.message);
            res.status(500).send('Error occurred while predicting');
        });
    });
});


app.use('/uploads', express.static(path.join(__dirname, 'uploads')));


app.listen(3000, () => {
    console.log('Server started on http://localhost:3000');
});
