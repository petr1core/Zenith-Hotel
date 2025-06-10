import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import apiService from "../services/apiService";

const RoomDetailPage = () => {
  const { id } = useParams(); // Получаем id из URL
  const [room, setRoom] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    apiService
      .getRoomById(id) // Загружаем данные о room
      .then((response) => {
        setRoom(response.data);
        setLoading(false);
      })
      .catch((err) => {
        setError("Failed to load room details");
        setLoading(false);
      });
  }, [id]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;
  if (!room) return <p>Room not found</p>;

  return (
    <div style={{ padding: "20px" }}>
      <h1>Room Details</h1>
      <table
        border="1"
        style={{ width: "100%", borderCollapse: "collapse", marginTop: "20px" }}
      >
        <thead>
          <tr>
            <th>ID</th>
            <th>Number</th>
            <th>Type</th>
            <th>Avaliability</th>
            <th>Charge</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>{room.guestId}</td>
            <td>{room.roomNumber}</td>
            <td>{room.roomType}</td>
            <td>{room.avaliability}</td>
            <td>{room.roomCharge}</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default RoomDetailPage;
