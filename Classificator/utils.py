from datetime import datetime
import re

def getTimestamp(str):
    datetime_object = datetime.strptime(str, '%d.%m.%Y %H:%M:%S')
    timestamp = datetime_object.timestamp()
    return timestamp

def splitRec(str):
    array = re.findall(r'\[([^\]]+)\]', str)
    return array