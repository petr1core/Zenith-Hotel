import React, { useEffect, useState } from "react";
import apiService from "../services/apiService";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Typography,
  CircularProgress,
  Alert
} from "@mui/material";
import dayjs from "dayjs";

var isSameOrBefore = require("dayjs/plugin/isSameOrBefore");
var isSameOrAfter = require("dayjs/plugin/isSameOrAfter");
dayjs.extend(isSameOrBefore);
dayjs.extend(isSameOrAfter);


const HomePage = () => {
  const [guests, setGuests] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [startDate, setStartDate] = useState(dayjs().startOf("day"));
  const [endDate, setEndDate] = useState(dayjs().add(14, "day").endOf("day"));
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);



  useEffect(() => {
    const fetchData = async () => {
      try {
        const [guestsResponse, bookingsResponse] = await Promise.all([
          apiService.getGuests(),
          apiService.getBookings(),
        ]);

        if (Array.isArray(guestsResponse.data)) {
          setGuests(guestsResponse.data);
        } else {
          console.error("Unexpected data structure for guests:", guestsResponse.data);
          setError("Unexpected data structure for guests");
        }

        if (Array.isArray(bookingsResponse.data)) {
          setBookings(bookingsResponse.data);

        } else {
          console.error("Unexpected data structure for bookings:", bookingsResponse.data);
          setError("Unexpected data structure for bookings");
        }

        setLoading(false);
      } catch (error) {
        console.error("Error fetching data:", error);
        setError("Error fetching data");
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  // useEffect(() => {
  //   console.log('Guests:', guests);
  //   console.log('Bookings:', bookings);
  // }, [guests, bookings]);

  // const fetchGuests = async () => {
  //   try {
  //     const response = await apiService.getGuests();
  //     setGuests(response.data);
  //   } catch (error) {
  //     console.error("Error fetching guests:", error);
  //   }
  // };

  // const fetchBookings = async () => {
  //   try {
  //     const response = await apiService.getBookings();
  //     setBookings(response.data);
  //   } catch (error) {
  //     console.error("Error fetching bookings:", error);
  //   }
  // };

  const getBookingsForGuestInRange = (id, startDate, endDate) => {
    return bookings.filter((booking) => {
      const bookingStart = dayjs(booking.checkInDate);
      const bookingEnd = dayjs(booking.checkOutDate);
      return (
        booking.guestId == id &&
        (bookingEnd.isSameOrAfter(startDate) || bookingStart.isSameOrBefore(endDate))
      );
    });
  };

  const getDaysInRange = (startDate, endDate) => {
    const days = [];
    let currentDate = dayjs(startDate);
    while (currentDate.isSameOrBefore(endDate)) {
      days.push(currentDate.format("YYYY-MM-DD"));
      currentDate = currentDate.add(1, "day");
    }
    return days;
  };

  const moveRangeForward = () => {
    setStartDate(startDate.add(15, "day"));
    setEndDate(endDate.add(15, "day"));
  };

  const moveRangeBackward = () => {
    setStartDate(startDate.subtract(15, "day"));
    setEndDate(endDate.subtract(15, "day"));
  };

  const days = getDaysInRange(startDate, endDate);

  if (loading) {
    return (
      <div style={{ padding: "20px", textAlign: "center" }}>
        <CircularProgress />
        <Typography variant="body1" gutterBottom>
          Loading...
        </Typography>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: "20px" }}>
        <Alert severity="error">{error}</Alert>
      </div>
    );
  }

  return (
    <div style={{ padding: "20px" }}>
      <Typography variant="h4" gutterBottom>
        Hotel Bookings Schedule
      </Typography>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          marginBottom: "20px",
        }}
      >
        <Button variant="contained" color="primary" onClick={moveRangeBackward}>
          Previous 15 Days
        </Button>
        <Typography variant="subtitle1">
          {startDate.format("YYYY-MM-DD")} - {endDate.format("YYYY-MM-DD")}
        </Typography>
        <Button variant="contained" color="primary" onClick={moveRangeForward}>
          Next 15 Days
        </Button>
      </div>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Guest Name</TableCell>
              {days.map((day, index) => (
                <TableCell key={`day-${index}`} align="center">
                  {day}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {guests.map((guest) => {
              if (!guest.id) {
                console.warn(`Guest without id:`, guest);
                return null; // Пропускаем гостя без id
              }
              const guestBookings = getBookingsForGuestInRange(
                guest.id,
                startDate,
                endDate
              );
              //console.log(guestBookings);
              return (
                <TableRow key={`guest-${guest.id}`}>
                  <TableCell>{`${guest.guestFirstname} ${guest.guestLastname}`}</TableCell>
                  {days.map((day, index) => {
                    const isBooked = guestBookings.some((booking) => {
                      const bookingStart = dayjs(booking.checkInDate);
                      const bookingEnd = dayjs(booking.checkOutDate);
                      const currentDay = dayjs(day);
                      //console.log(days);
                      return (
                        currentDay.isSameOrAfter(bookingStart) &&
                        currentDay.isSameOrBefore(bookingEnd)
                      );
                    });
                    return (
                      <TableCell
                        key={`guest-${guest.id}-day-${index}`}
                        align="center"
                        style={{ backgroundColor: isBooked ? "#dddddd" : "#ffffff" }}
                      >
                        {isBooked ? "Booked" : ""}
                      </TableCell>
                    );
                  })}
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </div>
  );
};

export default HomePage;