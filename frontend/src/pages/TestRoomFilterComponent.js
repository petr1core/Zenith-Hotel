// TestRoomFilterComponent.js
const TestRoomFilterComponent = () => {
    const [rooms, setRooms] = useState([]);
    const [filter, setFilter] = useState({ 
      minPrice: 100, 
      maxPrice: 500,
      roomType: 'Business'
    });
  
    const fetchRooms = async () => {
      const response = await apiService.get('/room/available', { params: filter });
      setRooms(response.data);
    };
  
    return (
      <div>
        <input 
          type="number" 
          value={filter.minPrice} 
          onChange={e => setFilter({...filter, minPrice: e.target.value})}
        />
        <input 
          type="number" 
          value={filter.maxPrice} 
          onChange={e => setFilter({...filter, maxPrice: e.target.value})}
        />
        <button onClick={fetchRooms}>Apply Filter</button>
        
        {rooms.map(room => (
          <div key={room.roomId}>{room.roomType} - {room.roomCharge}</div>
        ))}
      </div>
    );
  };

  export default TestRoomFilterComponent;