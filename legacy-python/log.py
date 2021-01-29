import logging

# Logging adapter class
class LoggingAdapter(logging.LoggerAdapter):
    def process(self, msg, kwargs):
        return '[%s] %s' % (self.extra['id'], msg), kwargs

# Setup logging
logging.basicConfig(level=logging.DEBUG,
                    datefmt='%Y-%m-%d %H:%M:%S',
                    format='%(asctime)s %(levelname)-4s %(message)s')

_logger = logging.getLogger(__name__)

# Create server logger adapter
serverLogger = LoggingAdapter(_logger, {'id': "SERVER"})

# Create adapter for a custom ID
def getAdapter(id):
    return LoggingAdapter(_logger, {"id": id})
