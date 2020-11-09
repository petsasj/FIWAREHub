import os

# Redis cache configuration
CACHE_CONFIG = {
    'CACHE_TYPE': 'redis',
    'CACHE_DEFAULT_TIMEOUT': 300,
    'CACHE_KEY_PREFIX': 'superset_',
    'CACHE_REDIS_HOST': 'thesis_redis',
    'CACHE_REDIS_PORT': 6379,
    'CACHE_REDIS_DB': 1,
    'CACHE_REDIS_URL': 'redis://thesis_redis:6379/1'}
	
# The SQLAlchemy connection string to your database backend
# This connection defines the path to the database that stores your
# superset metadata (slices, connections, tables, dashboards, ...).
# Note that the connection information to connect to the datasources
# you want to explore are managed directly in the web UI
POSTGRESHOST = os.getenv('POSTGRES_HOST', '')
SQLALCHEMY_DATABASE_URI = 'postgresql+psycopg2://superset:superset@' + POSTGRESHOST + ':5432/superset'
SQLALCHEMY_TRACK_MODIFICATIONS = True

# Flask App Builder configuration
# App secret key
SECRET_KEY = 'thisISaSECRET_1234'

# Row limit for superset data
ROW_LIMIT = 500000

# Set this API key to enable Mapbox visualizations
# Gets it from environment values of os, hence the import os
MAPBOX_API_KEY = os.getenv('MAPBOX_API_KEY', '')
ENABLE_JAVASCRIPT_CONTROLS = True