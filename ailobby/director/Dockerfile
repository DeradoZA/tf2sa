FROM python:3.12-slim

WORKDIR /app
COPY pyproject.toml ./
COPY ailobby_director ./ailobby_director
RUN pip install --no-cache-dir .

CMD ["python", "-m", "ailobby_director"]
