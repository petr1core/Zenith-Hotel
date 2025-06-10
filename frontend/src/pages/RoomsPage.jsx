// pages/RoomsPage.jsx
import React, { useState, useEffect } from "react";
import {
  Grid2,
  Card,
  CardMedia,
  CardContent,
  Typography,
  Button,
  Box,
  Rating,
} from "@mui/material";
import { Link } from "react-router-dom";
import apiService from "../services/apiService";
import RoomFilter from "../components/RoomFilter";

const RoomsPage = () => {
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filteredRooms, setFilteredRooms] = useState([]);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await apiService.getRooms();
        setRooms(response.data);
        setFilteredRooms(response.data);
        setLoading(false);
      } catch (error) {
        console.error("Error fetching rooms:", error);
        setLoading(false);
      }
    };

    fetchRooms();
  }, []);

  const handleFilter = (filters) => {
    const filtered = rooms.filter((room) => {
      return (
        room.roomCharge >= filters.minPrice &&
        room.roomCharge <= filters.maxPrice &&
        (filters.roomType === "all" || room.roomType === filters.roomType)
      );
    });
    setFilteredRooms(filtered);
  };

  if (loading) return <Typography>Loading rooms...</Typography>;

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Our Rooms
      </Typography>

      <RoomFilter onFilter={handleFilter} />

      <Grid2 container spacing={4}>
        {filteredRooms.map((room) => (
          <Grid2 item key={room.roomId} xs={12} sm={6} md={4}>
            <Card
              sx={{ height: "100%", display: "flex", flexDirection: "column" }}
            >
              <CardMedia
                component="img"
                height="200"
                image={`/images/rooms/${room.roomType.toLowerCase()}.jpg`}
                alt={room.roomType}
              />
              <CardContent sx={{ flexGrow: 1 }}>
                <Typography gutterBottom variant="h5" component="h2">
                  {room.roomType} Room
                </Typography>
                <Typography>Room #{room.roomNumber}</Typography>
                <Typography>Status: {room.availability}</Typography>
                <Box sx={{ display: "flex", alignItems: "center", mt: 1 }}>
                  <Rating value={4.5} precision={0.5} readOnly />
                  <Typography sx={{ ml: 1 }}>(24 reviews)</Typography>
                </Box>
                <Typography variant="h6" sx={{ mt: 1 }}>
                  ${room.roomCharge}{" "}
                  <Typography component="span" color="text.secondary">
                    / night
                  </Typography>
                </Typography>
              </CardContent>
              <Box sx={{ p: 2 }}>
                <Button
                  variant="contained"
                  fullWidth
                  component={Link}
                  to={`/book/${room.roomId}`}
                  disabled={room.availability !== "Free"}
                >
                  {room.availability === "Free" ? "Book Now" : "Unavailable"}
                </Button>
              </Box>
            </Card>
          </Grid2>
        ))}
      </Grid2>
    </Box>
  );
};

export default RoomsPage;
