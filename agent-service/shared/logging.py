import logging
from logging.handlers import RotatingFileHandler
# Configure logging
logger = logging.getLogger(__name__)
logger.setLevel(logging.DEBUG)  # Set to DEBUG for detailed logging

# Create a rotating file handler (max 5MB, keep 3 backups)
file_handler = RotatingFileHandler("app.log", maxBytes=5*1024*1024, backupCount=3)
file_handler.setLevel(logging.DEBUG)

# Create a console handler for real-time logging
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.DEBUG)

# Define a detailed log format
formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - [%(threadName)s] - %(message)s')
file_handler.setFormatter(formatter)
console_handler.setFormatter(formatter)

# Add handlers to the logger
logger.addHandler(file_handler)
logger.addHandler(console_handler)