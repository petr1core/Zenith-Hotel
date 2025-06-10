import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import apiService from "../services/apiService";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  Paper,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  Alert,
} from "@mui/material";
import dayjs from "dayjs";
import { v4 as uuidv4 } from "uuid";

var isSameOrAfter = require("dayjs/plugin/isSameOrAfter");
dayjs.extend(isSameOrAfter);

const BookingPage = () => {
  const [bookings, setBookings] = useState([]);
  const [guests, setGuests] = useState([]);
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [currentBooking, setCurrentBooking] = useState({
    bookingId: null,
    guestId: null,
    roomId: null,
    bookingStatus: "",
    checkInDate: "",
    checkOutDate: "",
  });
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [bookingsResponse, guestsResponse, roomsResponse] =
          await Promise.all([
            apiService.getBookings(),
            apiService.getGuests(),
            apiService.getRooms(),
          ]);

        if (Array.isArray(bookingsResponse.data)) {
          setBookings(bookingsResponse.data);
        } else {
          console.error(
            "Unexpected data structure for bookings:",
            bookingsResponse.data
          );
          setError("Unexpected data structure for bookings");
        }

        if (Array.isArray(guestsResponse.data)) {
          setGuests(guestsResponse.data);
        } else {
          console.error(
            "Unexpected data structure for guests:",
            guestsResponse.data
          );
          setError("Unexpected data structure for guests");
        }

        if (Array.isArray(roomsResponse.data)) {
          setRooms(roomsResponse.data);
        } else {
          console.error(
            "Unexpected data structure for rooms:",
            roomsResponse.data
          );
          setError("Unexpected data structure for rooms");
        }
        setLoading(false);
      } catch (error) {
        console.error("Error fetching data:", error); // post error
        setError("Error fetching data");
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  // useEffect(() => {
  //   console.log("Guests:", guests);
  //   console.log("Bookings:", bookings);
  //   console.log("Rooms:", rooms);
  // }, [guests, bookings]);

  const handleInputChange = (e) => {
    setCurrentBooking({ ...currentBooking, [e.target.name]: e.target.value });
  };

  const validateBooking = () => {
    const {
      bookingId,
      guestId,
      roomId,
      bookingStatus,
      checkInDate,
      checkOutDate,
    } = currentBooking;
    console.log("curr booking", currentBooking);
    if (
      !guestId ||
      !roomId ||
      !bookingStatus ||
      !checkInDate ||
      !checkOutDate
    ) {
      setError("Please fill in all fields");
      return false;
    }

    const checkIn = dayjs(checkInDate);
    const checkOut = dayjs(checkOutDate);

    if (!checkIn.isValid() || !checkOut.isValid()) {
      setError("Please enter valid dates");
      return false;
    }

    if (checkIn.isSameOrAfter(checkOut)) {
      setError("Check-in date must be before or equal to check-out date");
      return false;
    }

    // Проверка наличия свободной комнаты
    const room = rooms.find((r) => r.id == roomId);
    console.log("dasdasd", room, roomId);
    if (!room || !room.availability === "Free") {
      setError("Selected room is not available");
      return false;
    }

    // Проверка наличия гостя
    const guest = guests.find((g) => g.id == guestId);
    if (!guest) {
      setError("Selected guest does not exist");
      return false;
    }

    setError("");
    return true;
  };

  const handleAddBooking = () => {
    if (!validateBooking()) return;

    const newBooking = {
      id: uuidv4(), // Генерация UUID на клиенте
      guestId: currentBooking.guestId,
      roomId: currentBooking.roomId,
      bookingStatus: currentBooking.bookingStatus,
      checkInDate: dayjs(currentBooking.checkInDate).format('YYYY-MM-DD'),
      checkOutDate: dayjs(currentBooking.checkOutDate).format('YYYY-MM-DD'),
    };
    console.log("hhhhhhhhhh", newBooking);

    apiService
      .createBooking(newBooking)
      .then((response) => {
        console.log("Booking created:", response.data);
        setBookings([...bookings, response.data]); // Добавляем новое бронирование в список
        setCurrentBooking({
          id: null,
          guestId: null,
          roomId: null,
          bookingStatus: "",
          checkInDate: "",
          checkOutDate: "",
        }); // Очищаем форму
        setOpenDialog(false); // Закрываем диалоговое окно
      })
      .catch((error) => {
        console.error("Error creating Booking:", error);
        setError("Error creating booking");
      });
  };

  const handleEditBooking = (booking) => {
    setCurrentBooking(booking);
    setIsEditMode(true);
    setOpenDialog(true);
  };

  const handleUpdateBooking = () => {
    if (!validateBooking()) return;

    apiService
      .updateBooking(currentBooking.id, currentBooking)
      .then(() => {
        setBookings(
          bookings.map((b) =>
            b.id === currentBooking.id ? currentBooking : b
          )
        );
        setCurrentBooking({
          id: null,
          guestId: null,
          roomId: null,
          bookingStatus: "",
          checkInDate: "",
          checkOutDate: "",
        });
        setIsEditMode(false);
        setOpenDialog(false);
      })
      .catch((error) => {
        console.error("Error updating Booking:", error);
        setError("Error updating booking");
      });
  };

  const handleDeleteBooking = (id) => {
    if (window.confirm("Are you sure you want to delete this Booking?")) {
      apiService
        .deleteBooking(id)
        .then(() => setBookings(bookings.filter((b) => b.id !== id)))
        .catch((error) => {
          console.error("Error deleting Booking:", error);
          setError("Error deleting booking");
        });
    }
  };

  const getGuestName = (id) => {
    const guest = guests.find((g) => g.id === id);
    return guest
      ? `${guest.guestFirstname} ${guest.guestLastname}`
      : "Unknown id";
  };

  const getRoomNumber = (roomId) => {
    const room = rooms.find((r) => r.id === roomId);
    return room ? `${room.roomNumber}` : "Unknown roomId";
  };

  return (
    <div style={{ padding: "20px" }}>
      <Typography variant="h4" gutterBottom>
        Manage Bookings
      </Typography>
      <Button
        variant="contained"
        color="primary"
        onClick={() => setOpenDialog(true)}
      >
        Add New Booking
      </Button>
      {error && (
        <Alert severity="error" style={{ marginTop: "10px" }}>
          {error}
        </Alert>
      )}
      <TableContainer component={Paper} style={{ marginTop: 20 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>Guest Full Name</TableCell>
              <TableCell>Room Number</TableCell>
              <TableCell>Booking Status</TableCell>
              <TableCell>Check-In Date</TableCell>
              <TableCell>Check-Out Date</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {bookings.map((booking) => (
              <TableRow key={booking.id}>
                <TableCell>{booking.id}</TableCell>
                <TableCell>{getGuestName(booking.guestId)}</TableCell>
                <TableCell>{getRoomNumber(booking.roomId)}</TableCell>
                <TableCell>{booking.bookingStatus}</TableCell>
                <TableCell>{booking.checkInDate}</TableCell>
                <TableCell>{booking.checkOutDate}</TableCell>
                <TableCell>
                  <Link to={`/bookings/${booking.id}`}>
                    <Button size="small" color="primary">
                      View
                    </Button>
                  </Link>
                  <Button
                    size="small"
                    color="primary"
                    onClick={() => handleEditBooking(booking)}
                  >
                    Edit
                  </Button>
                  <Button
                    size="small"
                    color="secondary"
                    onClick={() => handleDeleteBooking(booking.id)}
                  >
                    Delete
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      {/* Диалоговое окно для добавления/редактирования бронирования */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        <DialogTitle>
          {isEditMode ? "Edit Booking" : "Add New Booking"}
        </DialogTitle>
        <DialogContent>
          <FormControl fullWidth margin="dense">
            <InputLabel id={`id-input-label`}>Guest ID</InputLabel>
            <Select
              labelId="guest-id-label"
              id="guest-id"
              name="guestId"
              value={currentBooking.guestId || ""}
              onChange={handleInputChange}
              label="Guest ID"
            >
              {guests.map((guest) => (
                <MenuItem key={`${guest.id}-menuItem`} value={guest.id}>
                  {getGuestName(guest.id)}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense">
            <InputLabel id="roomId-input-label">Room ID</InputLabel>
            <Select
              labelId="room-id-label"
              id="room-id"
              name="roomId"
              value={currentBooking.roomId || ""}
              onChange={handleInputChange}
              label="Room ID"
            >
              {rooms.map((room) => (
                <MenuItem key={room.id} value={room.id}>
                  {room.roomNumber}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Booking Status"
            name="bookingStatus"
            fullWidth
            value={currentBooking.bookingStatus}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            label="Check-In Date"
            name="checkInDate"
            type="date"
            fullWidth
            value={currentBooking.checkInDate}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            label="Check-Out Date"
            name="checkOutDate"
            type="date"
            fullWidth
            value={currentBooking.checkOutDate}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              setOpenDialog(false);
              setCurrentBooking({
                id: null,
                id: null,
                roomId: null,
                GuestFullName: "",
                RoomNumber: null,
                bookingStatus: "",
                checkInDate: "",
                checkOutDate: "",
              });
              setIsEditMode(false);
            }}
            color="primary"
          >
            Cancel
          </Button>
          <Button
            onClick={isEditMode ? handleUpdateBooking : handleAddBooking}
            color="primary"
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default BookingPage;
