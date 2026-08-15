namespace main.Service;

public class InsufficientStockException(string message) : Exception(message) { }